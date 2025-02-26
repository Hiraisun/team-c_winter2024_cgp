using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 射程内の敵全体に攻撃するコンポーネント
/// </summary>
public class UnitAttackAllEnemy : UnitAttackBase
{
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
            if(IsInRange(target)) //射程内?
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
            if(IsInRange(target)) //射程内なら
            {
                target.Damage(damage); //ダメージ
            }
        }
    }
}
