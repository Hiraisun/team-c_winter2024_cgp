using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 敵への範囲攻撃処理
/// </summary>
[RequireComponent(typeof(UnitBase))] // UnitBase必須やで
public abstract class UnitAttackBase : MonoBehaviour
{
    [SerializeField] protected UnitBase unitBase;
    [SerializeField] protected Animator animator;

    [SerializeField] protected List<Lane> attackableLaneList; // 攻撃可能レーン
    [SerializeField] protected float range = 1f; // 射程

    [SerializeField, Header("攻撃判定発生までの時間(秒)")] 
    protected float attackDuration = 1f;
    [SerializeField, Header("攻撃モーションの長さ(秒)")]
    protected float attackMotionDuration = 2f;

    void OnValidate()
    {
        // 自動アタッチ
        if(!Application.isPlaying)
        {
            unitBase = GetComponent<UnitBase>();
        }
    }

    void Update()
    {
        if(!unitBase.isBusy)
        {
            if (CanStartAttack())
            {
                StartCoroutine(AttackAction());
            }
        }
    }

    /// <summary>
    /// 攻撃開始条件
    /// </summary>
    protected abstract bool CanStartAttack();

    /// <summary>
    /// 攻撃処理
    /// </summary>
    protected abstract void Attack();


    // 攻撃処理コルーチン
    IEnumerator AttackAction()
    {
        unitBase.isBusy = true;
        if (animator != null) animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackDuration);
        Attack();
        yield return new WaitForSeconds(attackMotionDuration-attackDuration);
        unitBase.isBusy = false;
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
    /// 対象が攻撃可能位置にいるか
    /// </summary>
    protected bool isInRange(UnitBase target)
    {
        //attackLaneListに入っているかチェック
        if(!attackableLaneList.Contains(target.lane)) return false;

        //射程内にいるか
        float fromX = transform.position.x;                     //自分の位置
        float toX = fromX + range * unitBase.direction * -1;    //射程の先
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
