using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

// ユニットの制御、戦闘関連を扱うマネージャークラス
// ひとまずリアルタイム制を採用。
public class BattleManager1 : MonoBehaviour
{
    // シングルトン(簡易)
    public static BattleManager1 instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    [SerializeField] public SerializedDictionary<UnitBaseTest.Lane, float> LaneY;

    void Update()
    {
        
    }


    public void SummonUnit()
    {
        Debug.Log("Summon");
    }

    //デバッグ用
    void OnDrawGizmos()
    {
        //レーンの描画
        Gizmos.color = Color.red;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        foreach (var lane in LaneY)
        {
            //横線
            Gizmos.DrawLine(new Vector3(-10, lane.Value, 0), new Vector3(10, lane.Value, 0));
            //ラベル
            UnityEditor.Handles.Label(new Vector3(-10, lane.Value, 0), lane.Key.ToString(),style);
        }
    }

}
