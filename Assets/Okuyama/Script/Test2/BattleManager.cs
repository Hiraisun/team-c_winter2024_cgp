using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Lane{
    Ground,
    Sky
}

/// <summary>
/// バトル中のユニット,召喚などの管理
/// </summary>
public class BattleManager : MonoBehaviour
{
    [SerializeField] private float laneY;
    [SerializeField] private float spawnPosX;

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
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-20, laneY, 0), new Vector3(20, laneY, 0));

        //スポーン位置描画
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(-spawnPosX, laneY, 0), 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(spawnPosX, laneY, 0), 0.2f);
    }

}

