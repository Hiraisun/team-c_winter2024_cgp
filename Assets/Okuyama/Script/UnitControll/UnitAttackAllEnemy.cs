using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 射程内の敵全体に攻撃するコンポーネント
/// </summary>
public class UnitAttackAllEnemy : UnitAttackBase
{
    [SerializeField, Tooltip("攻撃開始射程")] 
    protected float attackStartRange = 1f;
    [SerializeField, Tooltip("射程距離(前方のみ)")] 
    protected float range = 1f;
    [SerializeField, Tooltip("与ダメージ")]
    private int damage = 10;

    /// <summary>
    /// 攻撃開始条件: 射程内に敵が一体でも存在すれば攻撃開始
    /// </summary>
    protected override bool CanStartAttack()
    {
        //ターゲット候補を問い合わせ
        List<UnitBase> targetList = unitBase.BattleManager.getEnemyUnitList(unitBase.Owner);
        
        //各候補について確認
        foreach (var target in targetList)
        {
            if(IsInRange(target, attackStartRange)) //射程内?
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 攻撃判定処理: 射程内の敵全員にダメージ
    /// </summary>
    protected override void Attack()
    {
        //ターゲット候補を問い合わせ
        List<UnitBase> targetList = unitBase.BattleManager.getEnemyUnitList(unitBase.Owner);
        
        //各候補について
        foreach (var target in targetList)
        {
            if(IsInRange(target, range)) //射程内なら
            {
                target.Damage(damage); //ダメージ
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        //攻撃開始範囲の描画
        Gizmos.color = Color.yellow;
        Vector3 Center = new Vector3(transform.position.x + attackStartRange * unitBase.direction * -0.5f, transform.position.y + 0.52f, 0);
        Vector3 Size = new Vector3(attackStartRange, 1, 1);
        Gizmos.DrawWireCube(Center, Size);

        //攻撃範囲の描画
        Gizmos.color = Color.red;
        Vector3 Center2 = new Vector3(transform.position.x + range * unitBase.direction * -0.5f, transform.position.y + 0.52f, 0);
        Vector3 Size2 = new Vector3(range, 1, 1);
        Gizmos.DrawWireCube(Center2, Size2);
    }
#endif
}
