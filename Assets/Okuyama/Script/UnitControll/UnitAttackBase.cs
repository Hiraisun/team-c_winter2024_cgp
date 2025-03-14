using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using System.Threading;

[Serializable]
public struct DamageInfo
{
    [HideInInspector] public UnitBase attacker;
    [HideInInspector] public float damage;
    public DamageType damageType;

    [SerializeField, Tooltip("与えるノックバック値 (耐性値に達するとノックバック)")]
    public float knockbackDamage;
}
public enum DamageType
{
    MELEE,  // 近接攻撃
    RANGED, // 遠距離攻撃
    THORN   // とげ
}

/// <summary>
/// 敵への攻撃行動を扱うコンポーネントの基底クラス
/// 攻撃行動は割り込み可能
/// </summary>
[DisallowMultipleComponent]
public abstract class UnitAttackBase : UnitActionBase
{
    [Header("攻撃")]

    [SerializeField, Tooltip("与ダメージ情報")]
    protected DamageInfo damageInfo;

    [SerializeField, Tooltip("攻撃モーション開始から判定発生までの遅延(秒)")] 
    protected float attackDelay = 1f;
    [SerializeField, Tooltip("攻撃モーション全体の長さ(秒)")]
    protected float attackMotionDuration = 2f;

    [SerializeField, Tooltip("攻撃可能レーンのリスト")] 
    protected List<Lane> attackableLaneList; // 攻撃可能レーン

    CancellationTokenSource cts; // Taskのキャンセル用トークンソース

    void Start()
    {
        damageInfo.attacker = UnitBase;
    }

    void Update()
    {
        if (UnitBase.UnitState != UnitState.MAIN) return; // メイン状態でないなら処理しない

        if (UnitBase.IsBusy) return; // 行動中なら処理しない

        if (CanStartAttack()) // 攻撃開始条件を満たしている
        {
            // 攻撃処理を開始
            // 割り込み時手動キャンセル, オブジェクト破棄時自動キャンセル
            cts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken(), destroyCancellationToken);
            AttackTask(cts.Token).Forget();
            UnitBase.Events.InvokeAttackStart();
        }
    }

    /// <summary>
    /// 攻撃処理の流れ
    /// </summary>
    async UniTask AttackTask(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested(); //キャンセルチェック

        UnitBase.StartAction(this); //アクション開始を宣言
        await UniTask.WaitForSeconds(attackDelay, cancellationToken: ct); //攻撃判定まで待機
        UnitBase.Reveal(); //隠密中止
        Attack(); // 攻撃判定処理
        await UniTask.WaitForSeconds(attackMotionDuration - attackDelay, cancellationToken: ct); //攻撃モーション終了まで待機
        UnitBase.Events.InvokeAttackEnd();
        UnitBase.FinishAction(this);
    }

    /// <summary>
    /// 攻撃中の割り込み
    /// </summary>
    public override bool InterruptAction()
    {
        cts.Cancel();
        cts.Dispose();
        UnitBase.FinishAction(this);
        UnitBase.Events.InvokeAttackEnd();
        return true;
    }

    /// <summary>
    /// 攻撃開始条件 継承先で記述する
    /// </summary>
    protected abstract bool CanStartAttack();

    /// <summary>
    /// 攻撃判定処理 継承先で記述する
    /// </summary>
    protected abstract void Attack();

    /// <summary>
    /// 対象がrange内にいるかを判定するためのメソッド
    /// </summary>
    protected bool IsInRange(UnitBase target, float range)
    {
        //対象のレーンが攻撃可能レーンか確認
        if(!attackableLaneList.Contains(target.Lane)) return false;

        //対象が隠密でないか確認
        if(target.IsHidden) return false;

        //射程内にいるか
        float fromX = transform.position.x;                     //自分の位置
        float toX = fromX + range * UnitBase.direction * -1;    //射程の先の地点
        float targetX = target.transform.position.x;            //相手の位置

        if (UnitBase.direction == 1) //左向き(味方)
        {
            if (toX <= targetX && targetX <= fromX)
            {
                return true;
            }
        }
        else //右向き(敵)
        {
            if (fromX <= targetX && targetX <= toX)
            {
                return true;
            }
        }
        return false;
    }

}
