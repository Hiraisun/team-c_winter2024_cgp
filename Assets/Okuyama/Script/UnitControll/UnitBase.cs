using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum OwnerType
{
    PLAYER,
    NPC,
}

/// <summary>
/// Unitには必ずアタッチする基本コンポーネント
/// イベントの受け渡しなどを行う。
/// 各種耐性など、受け身な情報はここに実装？
/// </summary>
[DisallowMultipleComponent] //複数アタッチ禁止
public class UnitBase : MonoBehaviour
{
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
    

    // 各種イベント
    private Action OnDeath;                  //死亡時
    private Action OnAttackStart;            //攻撃開始時
    private Action<UnitBase> OnDamageDealt;  //与ダメージ時 引数:target
    private Action<float> OnDamageReceived;  //被ダメージ時 引数:damage

    // HP
    [SerializeField, Tooltip("最大HP")]
    private float maxHP = 100;
    public float MaxHP { get { return maxHP; } }
    public float HP { get; private set; }

    // 向き : 移動などで乗算するためのパラメータ 
    // (PLAYERは1, ENEMYは-1)
    public float direction{
        get{
            return owner == OwnerType.PLAYER ? 1 : -1;
        }
    }

    void OnValidate()
    {
        // エディタ上でもNPC時の見た目反転
        transform.localScale = new Vector3(direction, 1, 1);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(BattleManager battleManager, OwnerType owner)
    {
        this.battleManager = battleManager;
        this.owner = owner;

        HP = MaxHP;
        battleManager.RegisterUnit(this); //battleManagerにユニットを登録

        //NPC時の見た目反転
        transform.localScale = new Vector3(direction, 1, 1);
    }

    /// <summary>
    /// このユニットにダメージを与える
    /// </summary>
    public void Damage(float damageValue)
    {
        if(damageValue < 0)
        {
            Debug.LogWarning("Damage: 負のダメージを受けた");
            return;
        }

        HP -= damageValue;

        if (HP <= 0)
        {
            //死亡時処理
            battleManager.DeRegisterUnit(this);
            OnDeath?.Invoke(); // 死亡イベント発火
            Destroy(gameObject);
        }

        // 被ダメージイベント発火
        OnDamageReceived?.Invoke(damageValue);
    }

    /// <summary>
    /// アクション実行開始通知
    /// 他の行動に割り込まれない行動を実行する場合はこれを呼ぶ。
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

    // イベントレジスタ-------------------------------------
    public void RegisterOnDeath(Action action)
    {
        OnDeath += action;
    }
    public void RegisterOnAttackStart(Action action)
    {
        OnAttackStart += action;
    }
    public void RegisterOnDamageDealt(Action<UnitBase> action)
    {
        OnDamageDealt += action;
    }
    public void RegisterOnDamageReceived(Action<float> action)
    {
        OnDamageReceived += action;
    }

    // 外部からイベント発火---------------------------------
    /// <summary>
    /// 攻撃開始イベントを発火する
    /// </summary>
    public void InvokeOnAttackStart()
    {
        OnAttackStart?.Invoke();
    }
    /// <summary>
    /// 与ダメージイベントを発火する
    /// </summary>
    public void InvokeOnDamageDealt(UnitBase target)
    {
        OnDamageDealt?.Invoke(target);
    }




    // デバッグ用
    void OnDrawGizmosSelected()
    {
        // 頭上にstateを表示
        string text;
        if(isBusy)
        {
            text = executionAction.GetType().Name;
        }
        else
        {
            text = "Idle";
        }
        var guiStyle = new GUIStyle {fontSize = 20, normal = {textColor = Color.black}};
        Handles.Label(transform.position + Vector3.up, text, guiStyle);

    }
}

