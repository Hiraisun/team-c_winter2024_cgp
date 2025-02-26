using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 最も近くにいる敵単体に攻撃するコンポーネント
/// </summary>
public class UnitAttackSingle : UnitAttackBase
{
    [SerializeField, Tooltip("与ダメージ")]
    private int damage = 10;

    /// <summary>
    /// 攻撃開始条件: 射程内に敵が一体でも存在すれば攻撃開始
    /// </summary>
    protected override bool CanStartAttack()
    {
        //ターゲット候補
        List<UnitBase> targetList = unitBase.BattleManager.getEnemyUnitList(unitBase.Owner);
        
        //各候補について確認
        foreach (var target in targetList)
        {
            if (IsInRange(target)) //射程内?
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 攻撃判定処理: 最も近い敵一体にダメージ
    /// </summary>
    protected override void Attack()
    {
        //ターゲット候補
        List<UnitBase> targetList = unitBase.BattleManager.getEnemyUnitList(unitBase.Owner);

        UnitBase nearestTarget = null;
        float distance = float.MaxValue;
        
        //各候補について確認し、最も近い敵を調べる
        foreach (var target in targetList)
        {
            if (IsInRange(target)) //射程内
            {
                // 敵との距離
                float targetDistance = Mathf.Abs(transform.position.x - target.transform.position.x);

                //最も近い敵を更新
                if (targetDistance < distance)
                {
                    nearestTarget = target;
                    distance = targetDistance;
                }
            }
        }
        
        //見つかったやつに攻撃
        if (nearestTarget != null)
        {
            nearestTarget.Damage(damage);
        }
    }

    
}
