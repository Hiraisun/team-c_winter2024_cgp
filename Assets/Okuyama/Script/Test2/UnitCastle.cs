using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 本拠地(城)の処理
/// 勝利/敗北判定
/// </summary>
[RequireComponent(typeof(UnitBase))]
public class UnitCastle : MonoBehaviour
{
    [SerializeField] private UnitBase unitBase;

    // 自動アタッチ
    void Reset()
    {
        unitBase = GetComponent<UnitBase>();
    }

    void Awake()
    {
        // 死亡時:勝敗判定
        unitBase.OnDeath += () =>
        {
            if(unitBase.unitType == UnitTYPE.PLAYER)
            {
                Debug.Log("敗北");
            }
            else
            {
                Debug.Log("勝利");
            }
        };
    }

    
}
