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
    private List<UnitBase> playerUnitList = new();
    private List<UnitBase> npcUnitList = new();

    void Awake()
    {
        // シーン開始時、既に存在するユニットの初期化
        foreach (var unit in FindObjectsOfType<UnitBase>())
        {
            unit.Initialize(this, unit.UnitType);
        }
    }


    /// <summary>
    /// ユニットを召喚する
    /// </summary>
    public void SummonUnit(GameObject unitPrefab, UnitTYPE type)
    {
        //TODO: Y軸を乱数でブレさせる
        Vector3 pos = new Vector3(type == UnitTYPE.PLAYER ? spawnPosX : -spawnPosX, laneY, 0);
        //召喚
        GameObject unitObj = Instantiate(unitPrefab, pos, Quaternion.identity, transform);
        UnitBase unit = unitObj.GetComponent<UnitBase>();

        //ユニット初期化
        unit.Initialize(this, type);
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

    /// <summary>
    /// 指定した派閥に敵対するユニットリストを取得
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
    /// 指定した派閥と同じ所属のユニットリストを取得
    /// </summary>
    public List<UnitBase> getAllyUnitList(UnitTYPE type)
    {
        if(type == UnitTYPE.PLAYER)
        {
            return playerUnitList;
        }
        else if(type == UnitTYPE.NPC)
        {
            return npcUnitList;
        }
        return null;
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

