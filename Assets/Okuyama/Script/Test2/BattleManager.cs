using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private StageData stageData;

    // (攻撃などの対象となる)ユニットのリスト
    private List<UnitBase> playerUnitList = new List<UnitBase>();
    private List<UnitBase> enemyUnitList = new List<UnitBase>();
    
    /// <summary>
    /// 敵対するユニットリストを取得
    /// </summary>
    public List<UnitBase> getEnemyUnitList(UnitTYPE type)
    {
        // コピーを返す
        return type == UnitTYPE.PLAYER ? new List<UnitBase>(enemyUnitList) : new List<UnitBase>(playerUnitList);
    }

    /// <summary>
    /// ユニットリストに追加
    /// </summary>
    public void addUnitList(UnitBase unit)
    {
        if(unit.unitType == UnitTYPE.PLAYER)
        {
            playerUnitList.Add(unit);
        }
        else
        {
            enemyUnitList.Add(unit);
        }
    }

    /// <summary>
    /// ユニットリストから削除
    /// </summary>
    public void removeUnitList(UnitBase unit)
    {
        if (unit.unitType == UnitTYPE.PLAYER)
        {
            playerUnitList.Remove(unit);
        }
        else
        {
            enemyUnitList.Remove(unit);
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
