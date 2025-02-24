using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 本拠地(城)の処理
/// 勝利/敗北判定
/// </summary>
public class UnitCastle : UnitActionBase
{
    // 死亡時処理をオーバーライド
    protected override void OnDeath()
    {
        if (unitBase.Owner == OwnerType.PLAYER)
        {
            Debug.Log("敗北");
        }
        else
        {
            Debug.Log("勝利");
        }
    }

}
