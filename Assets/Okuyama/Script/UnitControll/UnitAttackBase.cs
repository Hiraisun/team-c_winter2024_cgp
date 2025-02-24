using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;

/// <summary>
/// 敵への攻撃行動を扱うコンポーネントの基底クラス
/// </summary>
public abstract class UnitAttackBase : UnitActionBase
{
    [SerializeField] protected Animator animator; // TODO:Animator制御は要検討

    [SerializeField, Tooltip("攻撃可能レーンのリスト")] 
    protected List<Lane> attackableLaneList; // 攻撃可能レーン
    [SerializeField, Tooltip("射程距離(前方のみ)")] 
    protected float range = 1f; // 射程

    [SerializeField, Tooltip("攻撃モーション開始から判定発生までの遅延(秒)")] 
    protected float attackDelay = 1f;
    [SerializeField, Tooltip("攻撃モーション全体の長さ(秒)")]
    protected float attackMotionDuration = 2f;

    void Update()
    {
        if (!unitBase.IsBusy) // 他のアクション中でない
        {
            if (CanStartAttack()) // 攻撃開始条件を満たしている
            {
                // 攻撃処理コルーチンを開始
                StartCoroutine(AttackAction());
            }
        }

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
    /// 攻撃処理コルーチン
    /// </summary>
    IEnumerator AttackAction()
    {
        unitBase.StartAction(this); //アクション開始を宣言

        if (animator != null) animator.SetTrigger("Attack"); //攻撃開始アニメーション TODO:要検討
        yield return new WaitForSeconds(attackDelay); //攻撃判定まで待機
        Attack(); // 攻撃判定処理
        yield return new WaitForSeconds(attackMotionDuration-attackDelay); //モーション終了待機
        unitBase.FinishAction(this);
    }

    void OnDrawGizmosSelected()
    {
        //攻撃範囲の描画
        Gizmos.color = Color.red;

        Vector3 Center = new Vector3(transform.position.x + range * unitBase.direction * -0.5f, transform.position.y + 0.52f, 0);
        Vector3 Size = new Vector3(range, 1, 1);
        Gizmos.DrawWireCube(Center, Size);
        
    }

    /// <summary>
    /// 対象が攻撃可能位置にいるかを判定する
    /// </summary>
    protected bool isInRange(UnitBase target)
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
