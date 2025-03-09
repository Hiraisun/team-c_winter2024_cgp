using System;
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

    [SerializeField]
    private UI_BuffIndicator buffIndicator;

    // (攻撃などの対象となる)ユニットのリスト
    private List<UnitBase> playerUnitList = new();
    private List<UnitBase> npcUnitList = new();

    // 待機中のバフ
    private Dictionary<OwnerType, CardEffectBuffBase> reserveBuffs = new();

    // 召喚完了イベント
    private Action<OwnerType> OnSummonComplete;
    public void AddOnSummonCompleteListener(Action<OwnerType> listener) => OnSummonComplete += listener;

    void Start()
    {
        // シーン開始時、既に存在するユニットの初期化
        foreach (var unit in FindObjectsOfType<UnitBase>())
        {
            unit.Initialize(this, unit.Owner);
            unit.Summon().Forget();
        }
    }

    /// <summary>
    /// バフを予約する
    /// </summary>
    public void ReserveBuff(CardEffectBuffBase buff, OwnerType owner)
    {
        reserveBuffs[owner] = buff;

        // プレイヤーの場合、UIにバフを表示
        if(owner == OwnerType.PLAYER)
        {
            buffIndicator.Activate(buff.BuffColor);
        }
    }


    /// <summary>
    /// ユニットを召喚する
    /// </summary>
    public void SummonUnit(GameObject unitPrefab, OwnerType owner)
    {
        float y = UnityEngine.Random.Range(laneMinY, laneMaxY);
        float x = owner == OwnerType.PLAYER ? spawnPosX : -spawnPosX;
        float z = laneMaxY - y; // 出現Y座標と表示順を対応
        
        //召喚
        GameObject unitObj = Instantiate(unitPrefab, new Vector3(x, y, z), Quaternion.identity, transform);
        UnitBase unit = unitObj.GetComponent<UnitBase>();

        //ユニット初期化
        unit.Initialize(this, owner);

        //バフ適用
        if(reserveBuffs.ContainsKey(owner))
        {
            reserveBuffs[owner].Buff(unitObj, unit);
            reserveBuffs.Remove(owner);
        }
        // 召喚完了を監視
        unit.Events.AddOnSummonCompleteListener(() => OnSummonComplete?.Invoke(owner));

        unit.Summon().Forget();
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
    public List<UnitBase> GetEnemyUnitList(OwnerType type)
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
    public List<UnitBase> GetAllyUnitList(OwnerType type)
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

#if UNITY_EDITOR
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
#endif
}

