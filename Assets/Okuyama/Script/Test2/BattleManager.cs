using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private StageData stageData;

    // (攻撃などの対象となる)ユニットのリスト
    private List<UnitBase> playerUnitList = new List<UnitBase>();
    private List<UnitBase> npcUnitList = new List<UnitBase>();
    
    /// <summary>
    /// 指定したタイプに敵対するユニットリストを取得
    /// </summary>
    public List<UnitBase> getEnemyUnitList(UnitTYPE type)
    {
        if(type == UnitTYPE.PLAYER)
        {
            return npcUnitList;
        }
        else if(type == UnitTYPE.NPC)
        {
            return playerUnitList;
        }
        return null;
    }

    /// <summary>
    /// ユニットを登録する
    /// 登録されたユニットは攻撃対象になるなど
    /// </summary>
    public void RegisterUnit(UnitBase unit)
    {
        if(unit.UnitType == UnitTYPE.PLAYER)
        {
            playerUnitList.Add(unit);
        }
        else if(unit.UnitType == UnitTYPE.NPC)
        {
            npcUnitList.Add(unit);
        }
    }

    /// <summary>
    /// ユニットを登録解除
    /// </summary>
    public void DeRegisterUnit(UnitBase unit)
    {
        if(unit.UnitType == UnitTYPE.PLAYER)
        {
            playerUnitList.Remove(unit);
        }
        else if(unit.UnitType == UnitTYPE.NPC)
        {
            npcUnitList.Remove(unit);
        }
    }

    //デバッグ用
    void OnDrawGizmos()
    {
        //レーンの描画
        Gizmos.color = Color.red;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        foreach (var lane in stageData.LaneParams)
        {
            //横線
            Gizmos.DrawLine(new Vector3(-10, lane.PosY, 0), new Vector3(10, lane.PosY, 0));
            //ラベル
            UnityEditor.Handles.Label(new Vector3(-10, lane.PosY, 0), lane.lane.ToString(),style);
        }

        //スポーン位置
        Gizmos.DrawSphere(new Vector3(stageData.SpawnPosX, stageData.LaneParams[0].PosY, 0), 0.1f);
        Gizmos.DrawSphere(new Vector3(-stageData.SpawnPosX, stageData.LaneParams[0].PosY, 0), 0.1f);
    }

}
