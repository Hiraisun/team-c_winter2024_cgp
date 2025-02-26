using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 敵への攻撃行動を扱うコンポーネントの基底クラス
/// 攻撃行動は割り込み可能
/// </summary>
public abstract class UnitAttackBase : UnitActionBase
{
    [Header("攻撃")]
    [SerializeField] protected Animator animator; // TODO:Animator制御は要検討

    [SerializeField, Tooltip("攻撃モーション開始から判定発生までの遅延(秒)")] 
    protected float attackDelay = 1f;
    [SerializeField, Tooltip("攻撃モーション全体の長さ(秒)")]
    protected float attackMotionDuration = 2f;

    [SerializeField, Tooltip("攻撃可能レーンのリスト")] 
    protected List<Lane> attackableLaneList; // 攻撃可能レーン

    CancellationTokenSource cts;
    CancellationToken ct;

    void Update()
    {
        if (!unitBase.IsBusy) // 他のアクション中でない
        {
            if (CanStartAttack()) // 攻撃開始条件を満たしている
            {
                // 攻撃処理を開始
                cts = new();
                ct = cts.Token;
                AttackTask(ct).Forget();
            }
        }
    }

    /// <summary>
    /// 攻撃処理の流れ
    /// </summary>
    async UniTask AttackTask(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested(); //キャンセルチェック

        unitBase.StartAction(this); //アクション開始を宣言
        if (animator != null) animator.SetTrigger("Attack"); //攻撃開始アニメーション TODO:要検討
        await UniTask.WaitForSeconds(attackDelay, cancellationToken: ct); //攻撃判定まで待機
        Attack(); // 攻撃判定処理
        await UniTask.WaitForSeconds(attackMotionDuration - attackDelay, cancellationToken: ct); //攻撃モーション終了まで待機
        unitBase.FinishAction(this);
    }

    /// <summary>
    /// 攻撃中の割り込み
    /// </summary>
    public override bool InterruptAction()
    {
        cts.Cancel();
        cts.Dispose();
        unitBase.FinishAction(this);
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

    // destory時にtaskをキャンセル
    void OnDestroy()
    {
        if (cts != null && !cts.IsCancellationRequested)
        {
            cts.Cancel();
            cts.Dispose();
        }
    }


    /// <summary>
    /// 対象がrange内にいるかを判定するためのメソッド
    /// </summary>
    protected bool IsInRange(UnitBase target, float range)
    {
        //対象のレーンが攻撃可能レーンか確認
        if(!attackableLaneList.Contains(target.Lane)) return false;

        //射程内にいるか
        float fromX = transform.position.x;                     //自分の位置
        float toX = fromX + range * unitBase.direction * -1;    //射程の先の地点
        float targetX = target.transform.position.x;            //相手の位置

        if (unitBase.direction == 1) //左向き(味方)
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
