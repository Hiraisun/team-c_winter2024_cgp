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

    // 最も近い敵に定数ダメージ
    protected override void Attack()
    {
        List<UnitBase> targetList = unitBase.battleManager.getEnemyUnitList(unitBase.unitType);
        UnitBase nearestTarget = null;
        float distance = float.MaxValue;
        
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
                        //ターゲット更新
                        float d = Mathf.Abs(targetX - transform.position.x);
                        if (d < distance)
                        {
                            distance = d;
                            nearestTarget = target;
                        }
                    }
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
