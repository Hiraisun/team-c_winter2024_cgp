using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackAllEnemy : UnitAttackBase
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
            if(isInRange(target))
            {
                return true;
            }
        }
        return false;
    }

    // 射程内の敵全員に定数ダメージ
    protected override void Attack()
    {
        List<UnitBase> targetList = unitBase.battleManager.getEnemyUnitList(unitBase.unitType);
        
        //各候補について
        foreach (var target in targetList)
        {
            if(isInRange(target))
            {
                // 対象全員にダメージ
                target.Damage(damage);
            }
        }
    }
}
