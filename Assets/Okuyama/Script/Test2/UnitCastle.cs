using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 本拠地(城)の処理
/// 勝利/敗北判定
/// </summary>
public class UnitCastle : MonoBehaviour
{
    [SerializeField] private UnitBase unitBase;

    void OnValidate()
    {
        // 自動アタッチ
        if(!Application.isPlaying)
        {
            unitBase = GetComponent<UnitBase>();
        }
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
