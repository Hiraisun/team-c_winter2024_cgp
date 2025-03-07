using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    [SerializeField, Tooltip("ユニットの召喚Y座標 最大値")] 
    private float laneMaxY;
    [SerializeField, Tooltip("ユニットの召喚Y座標 最小値")]
    private float laneMinY;
    [SerializeField, Tooltip("召喚X座標, プレイヤーとNPCで左右対称")] 
    private float spawnPosX;

    // (攻撃などの対象となる)ユニットのリスト
    private List<UnitBase> playerUnitList = new();
    private List<UnitBase> npcUnitList = new();

    void Awake()
    {
        // シーン開始時、既に存在するユニットの初期化
        foreach (var unit in FindObjectsOfType<UnitBase>())
        {
            unit.Initialize(this, unit.Owner);
        }
    }


    /// <summary>
    /// ユニットを召喚する
    /// </summary>
    public void SummonUnit(GameObject unitPrefab, OwnerType type)
    {
        float y = Random.Range(laneMinY, laneMaxY);
        float x = type == OwnerType.PLAYER ? spawnPosX : -spawnPosX;
        float z = laneMaxY - y; // 出現Y座標と表示順を対応
        
        //召喚
        GameObject unitObj = Instantiate(unitPrefab, new Vector3(x, y, z), Quaternion.identity, transform);
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
        if(unit.Owner == OwnerType.PLAYER)
        {
            playerUnitList.Add(unit);
        }
        else if(unit.Owner == OwnerType.NPC)
        {
            npcUnitList.Add(unit);
        }
    }

    /// <summary>
    /// ユニットを登録解除
    /// </summary>
    public void DeRegisterUnit(UnitBase unit)
    {
        if(unit.Owner == OwnerType.PLAYER)
        {
            playerUnitList.Remove(unit);
        }
        else if(unit.Owner == OwnerType.NPC)
        {
            npcUnitList.Remove(unit);
        }
    }

    /// <summary>
    /// 指定した派閥に敵対するユニットリストを取得
    /// </summary>
    public List<UnitBase> getEnemyUnitList(OwnerType type)
    {
        if(type == OwnerType.PLAYER)
        {
            return new List<UnitBase>(npcUnitList);
        }
        else if(type == OwnerType.NPC)
        {
            return new List<UnitBase>(playerUnitList);
        }
        return null;
    }

    /// <summary>
    /// 指定した派閥と同じ所属のユニットリストを取得
    /// </summary>
    public List<UnitBase> getAllyUnitList(OwnerType type)
    {
        if(type == OwnerType.PLAYER)
        {
            return new List<UnitBase>(playerUnitList);
        }
        else if(type == OwnerType.NPC)
        {
            return new List<UnitBase>(npcUnitList);
        }
        return null;
    }

    //調整用
    void OnDrawGizmosSelected()
    {
        //レーンの描画
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-20, laneMaxY, 0), new Vector3(20, laneMaxY, 0));
        Gizmos.DrawLine(new Vector3(-20, laneMinY, 0), new Vector3(20, laneMinY, 0));

        //スポーン位置描画
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(spawnPosX, laneMaxY, 0), new Vector3(spawnPosX, laneMinY, 0));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-spawnPosX, laneMaxY, 0), new Vector3(-spawnPosX, laneMinY, 0));
    }

}

