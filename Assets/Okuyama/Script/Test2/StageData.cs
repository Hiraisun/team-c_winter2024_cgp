using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ステージに関する、スクリプトに影響を与える情報を扱う
/// </summary>
[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObjects/EnumData")]
public class StageData : ScriptableObject
{

    [Serializable]
    public class LaneParam
    {
        public Lane lane;
        public float PosY;
    }

    public List<LaneParam> LaneParams;

    // ユニットの出現X座標 (正負反転で敵の出現位置)
    public float SpawnPosX;
}
public enum Lane
{
    Ground,
    Sky,
}
