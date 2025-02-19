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
            //レーンごとにチェック
            foreach (var laneRange in attackRangeList)
            {
                if (target.lane == laneRange.lane)
                {
                    float fromX = transform.position.x;
                    float toX = fromX + laneRange.range * unitBase.direction * -1;
                    float targetX = target.transform.position.x;

                    fromX *= unitBase.direction;
                    toX *= unitBase.direction;
                    targetX *= unitBase.direction;

                    if (toX <= targetX && targetX <= fromX)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // 射程内の敵全員に定数ダメージ
    protected override void Attack()
    {
        List<UnitBase> targetList = unitBase.battleManager.getEnemyUnitList(unitBase.unitType);
        List<UnitBase> targetListInAttackRange = new List<UnitBase>();
        
        //各候補について
        foreach (var target in targetList)
        {
            //レーンごとにチェック
            foreach (var laneRange in attackRangeList)
            {
                if (target.lane == laneRange.lane)
                {
                    float fromX = transform.position.x;
                    float toX = fromX + laneRange.range * unitBase.direction * -1;
                    float targetX = target.transform.position.x;

                    fromX *= unitBase.direction;
                    toX *= unitBase.direction;
                    targetX *= unitBase.direction;

                    if (toX <= targetX && targetX <= fromX)
                    {
                        targetListInAttackRange.Add(target);
                    }
                }
            }
        }
        
        //攻撃
        foreach (var target in targetListInAttackRange)
        {
            target.Damage(damage);
        }
    }
}
