using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public enum OwnerType
{
    PLAYER,
    NPC,
}

public enum UnitState
{
    SUMMON,
    MAIN,
    DEAD,
}

/// <summary>
/// Unitには必ずアタッチする基本コンポーネント
/// イベントの受け渡しなどを行う。
/// 各種耐性など、受け身な情報はここに実装？
/// </summary>
[DisallowMultipleComponent] //複数アタッチ禁止
public class UnitBase : MonoBehaviour
{
    [SerializeField, Tooltip("反転用オブジェクト")]
    private Transform flipObject;

    [Header("ユニット基本情報")]
    private BattleManager battleManager;
    public BattleManager BattleManager { get { return battleManager; } }

    [SerializeField, Tooltip("所属 : PLAYER or NPC")]
    private OwnerType owner; // PLAYER or NPC
    public OwnerType Owner {get { return owner; } }

    [SerializeField, Tooltip("レーン : 地上 or 空中")]
    private Lane lane; // 所属レーン
    public Lane Lane {get { return lane; } }

    private bool isBusy = false; // 何らかのアクション中か
    public bool IsBusy { get { return isBusy; } }
    private UnitActionBase executionAction; // 実行中のアクションコンポーネント

    // ユニット状態 
    private UnitState unitState = UnitState.SUMMON;
    public UnitState UnitState { get { return unitState; } }

    // 向き : 移動などで乗算するためのパラメータ 
    // (PLAYERは1, ENEMYは-1)
    public float direction{
        get{
            return owner == OwnerType.PLAYER ? 1 : -1;
        }
    }

    [SerializeField, Tooltip("最大HP")]
    private float maxHP = 100;
    public float MaxHP { get { return maxHP; } }
    public float HP { get; private set; }

    [SerializeField, Tooltip("攻撃力")]
    private float attackPower = 30;
    public float AttackPower { get { return attackPower; } }

    
    /// <summary>
    /// イベント管理用のクラス
    /// </summary>
    public class UnitEvents
    {
        // 初期化時
        private event Func<UniTask> OnSummon;
        public void AddOnSummonListener(Func<UniTask> listener) => OnSummon += listener;
        public async UniTask InvokeSummon()
        {
            if (OnSummon != null)
            {
                await UniTask.WhenAll(OnSummon.GetInvocationList().Cast<Func<UniTask>>().Select(d => d.Invoke()));
            }
        }

        // 初期化完了時
        private Action OnSummonComplete;
        public void AddOnSummonCompleteListener(Action listener) => OnSummonComplete += listener;
        public void InvokeSummonComplete() => OnSummonComplete?.Invoke();

        // 攻撃開始時 攻撃コンポーネントが発火する
        private Action OnAttackStart;
        public void AddOnAttackStartListener(Action listener) => OnAttackStart += listener;
        public void InvokeAttackStart() => OnAttackStart?.Invoke();

        // 与ダメージ時
        private Action<UnitBase> OnDamageDealt;
        public void AddOnDamageDealtListener(Action<UnitBase> listener) => OnDamageDealt += listener;
        public void InvokeDamageDealt(UnitBase target) => OnDamageDealt?.Invoke(target);
        
        // 被ダメージ時
        private Action<DamageInfo> OnDamageReceived;
        public void AddOnDamageReceivedListener(Action<DamageInfo> listener) => OnDamageReceived += listener;
        public void InvokeDamageReceived(DamageInfo damageInfo) => OnDamageReceived?.Invoke(damageInfo);

        // 死亡時
        private event Func<UniTask> OnDeath;
        public void AddOnDeathListener(Func<UniTask> listener) => OnDeath += listener;
        public async UniTask InvokeDeath()
        {
            if (OnDeath != null)
            {
                await UniTask.WhenAll(OnDeath.GetInvocationList().Cast<Func<UniTask>>().Select(d => d.Invoke()));
            }
        }
    }
    public UnitEvents Events { get; private set; } = new UnitEvents();




    

    void OnValidate()
    {
        // エディタ上でもNPC時の見た目反転
        if(flipObject != null) flipObject.localScale = new Vector3(direction, 1, 1);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(BattleManager battleManager, OwnerType owner)
    {
        this.battleManager = battleManager;
        this.owner = owner;

        HP = MaxHP;

        //NPC時の見た目反転
        if(flipObject != null) flipObject.localScale = new Vector3(direction, 1, 1);

        Summon().Forget();
    }
    // 召喚状態で待つ
    private async UniTask Summon(){
        unitState = UnitState.SUMMON; // 各アクションの初期化処理を待つ
        await Events.InvokeSummon();

        // 初期化完了、行動開始
        unitState = UnitState.MAIN;
        battleManager.RegisterUnit(this); //battleManagerにユニットを登録
        Events.InvokeSummonComplete();
    }

    /// <summary>
    /// このユニットにダメージを与える
    /// </summary>
    public void Damage(DamageInfo damageInfo)
    {
        if(damageInfo.damage < 0)
        {
            Debug.LogWarning("Damage: 負のダメージを受けた");
            return;
        }

        HP -= damageInfo.damage;
        // 被ダメージイベント発火
        Events.InvokeDamageReceived(damageInfo);

        if (HP <= 0)
        {
            //死亡
            Death().Forget();
        }
    }

    /// <summary>
    /// 死亡時処理
    /// </summary>
    private async UniTask Death(){
        InterruptAction(); // 行動中断
        battleManager.DeRegisterUnit(this);
        await Events.InvokeDeath();
        Destroy(gameObject);
    }

    /// <summary>
    /// 占有アクション実行開始通知
    /// 他の行動に割り込まれない行動を実行する場合はこれを呼ぶ。
    /// !! 占有する場合はInterruptActionを必ず実装すること !!
    /// </summary>
    public void StartAction(UnitActionBase unitActionBase)
    {
        isBusy = true;
        executionAction = unitActionBase;
    }
    /// <summary>
    /// アクション終了通知
    /// 占有アクション終了時には必ずこれを呼ぶ。
    /// </summary>
    public void FinishAction(UnitActionBase unitActionBase)
    {
        if(executionAction == unitActionBase)
        {
            isBusy = false;
            executionAction = null;
        }
        else
        {
            Debug.LogWarning("EndAction: 実行中のアクションと異なるアクションが終了しようとしています。: " + unitActionBase);
        }
    }

    /// <summary>
    /// 他の行動をキャンセルして割り込む
    /// trueならば新たに行動してOK, falseなら割り込み不可
    /// 例 if(unitBase.InterruptAction()) StartAction();
    /// </summary>
    public bool InterruptAction()
    {
        if(isBusy)
        {
            // 実行中のアクションに判断を任せる
            return executionAction.InterruptAction();
        }
        else{
            return true; // 何も行動していないので行動可
        }
    }

    // 外部からイベント発火---------------------------------
    /// <summary>
    /// 攻撃開始イベントを発火する
    /// </summary>
    public void InvokeOnAttackStart()
    {
        Events.InvokeAttackStart();
    }
    /// <summary>
    /// 与ダメージイベントを発火する
    /// </summary>
    public void InvokeOnDamageDealt(UnitBase target)
    {
        Events.InvokeDamageDealt(target);
    }



#if UNITY_EDITOR
    // デバッグ用
    void OnDrawGizmosSelected()
    {
        // 頭上に実行中アクションを表示
        // isBusyのときは赤
        string text;
        var guiStyle = new GUIStyle {fontSize = 20};
        if(isBusy)
        {
            text = executionAction.GetType().Name;
            guiStyle.normal.textColor = Color.red;
        }
        else
        {
            text = "Idle";
            guiStyle.normal.textColor = Color.black;
        }
        if(owner == OwnerType.NPC) guiStyle.alignment = TextAnchor.UpperRight;
        Handles.Label(transform.position + Vector3.up, text, guiStyle);

        // 頭上にUnitStateを表示
        string stateText = unitState.ToString();
        guiStyle.normal.textColor = Color.black;
        if(owner == OwnerType.NPC) guiStyle.alignment = TextAnchor.UpperRight;
        Handles.Label(transform.position + Vector3.up * 0.5f, stateText, guiStyle);
    }
#endif
}

