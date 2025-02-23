using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 最も近い敵に単体攻撃する機能
/// </summary>
public class UnitAttackSingle : UnitAttackBase
{
    [SerializeField] private int damage = 10;

    // 射程内に敵が一体でも存在すれば攻撃開始
    protected override bool CanStartAttack()
    {
        //ターゲット候補
        List<UnitBase> targetList = unitBase.battleManager.getEnemyUnitList(unitBase.unitType);
        
        //各候補について
        foreach (var target in targetList)
        {
            if (isInRange(target))
            {
                return true;
            }
        }
        return false;
    }

    // 最も近い敵に定数ダメージ
    protected override void Attack()
    {
        List<UnitBase> targetList = unitBase.battleManager.getEnemyUnitList(unitBase.unitType);
        UnitBase nearestTarget = null;
        float distance = float.MaxValue;
        
        //各候補について
        foreach (var target in targetList)
        {
            if (isInRange(target))
            {
                float targetDistance = Mathf.Abs(transform.position.x - target.transform.position.x);

                //最も近い敵を更新
                if (targetDistance < distance)
                {
                    nearestTarget = target;
                    distance = targetDistance;
                }
            }
        }
        
        //攻撃
        if (nearestTarget != null)
        {
            nearestTarget.Damage(damage);
        }
    }

    
}
