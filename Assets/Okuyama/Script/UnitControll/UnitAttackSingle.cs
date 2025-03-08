using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 最も近くにいる敵単体に攻撃するコンポーネント
/// </summary>
public class UnitAttackSingle : UnitAttackBase
{
    [SerializeField, Tooltip("射程距離(前方のみ)")] 
    protected float range = 1f; // 射程

    /// <summary>
    /// 攻撃開始条件: 射程内に敵が一体でも存在すれば攻撃開始
    /// </summary>
    protected override bool CanStartAttack()
    {
        //ターゲット候補
        List<UnitBase> targetList = UnitBase.BattleManager.getEnemyUnitList(UnitBase.Owner);
        
        //各候補について確認
        foreach (var target in targetList)
        {
            if (IsInRange(target, range)) //射程内?
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
        List<UnitBase> targetList = UnitBase.BattleManager.getEnemyUnitList(UnitBase.Owner);

        UnitBase nearestTarget = null;
        float distance = float.MaxValue;
        
        //各候補について確認し、最も近い敵を調べる
        foreach (var target in targetList)
        {
            if (IsInRange(target, range)) //射程内?
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
            damageInfo.damage = UnitBase.AttackPower;
            nearestTarget.Damage(damageInfo);
        }
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        //攻撃範囲の描画
        Gizmos.color = Color.red;

        Vector3 Center = new Vector3(transform.position.x + range * UnitBase.direction * -0.5f, transform.position.y + 0.52f, 0);
        Vector3 Size = new Vector3(range, 1, 1);
        Gizmos.DrawWireCube(Center, Size);
    }
#endif

    
}
