using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 基本的な攻撃処理
/// </summary>
public class UnitAttackNormal : MonoBehaviour
{
    [Serializable]
    public class LaneRange
    {
        public Lane lane;
        public float range;
    }

    // 攻撃範囲リスト (各レーン)
    [SerializeField] private List<LaneRange> attackRangeList;


    
}
