using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private StageData stageData;

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
