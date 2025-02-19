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

    [Serializable]
    public class LaneRange
    {
        public Lane lane;
        public float range;
    }

    // 攻撃範囲リスト (各レーン)
    [SerializeField] protected List<LaneRange> attackRangeList;

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
        foreach (var laneRange in attackRangeList)
        {
            float Y = unitBase.stageData.LaneParams.Find(x => x.lane == laneRange.lane).PosY + 0.55f;
            Vector3 Center = new Vector3(transform.position.x + laneRange.range * unitBase.direction * -0.5f, Y, 0);
            Vector3 Size = new Vector3(laneRange.range, 1, 1);
            Gizmos.DrawWireCube(Center, Size);
        }
    }
}
