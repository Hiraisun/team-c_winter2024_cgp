using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ユニットの頭上にHPバーを表示するコンポーネント
/// </summary>
public class UnitHpBar : UnitActionBase
{
    [Header("HPバー")]
    [SerializeField] private GameObject hpBarPrefabAllay; // HPバーのプレハブ
    [SerializeField] private GameObject hpBarParentEnemy; // 

    [SerializeField] private float height = 0.05f; // HPバーの高さ

    private GameObject hpBarObj;
    private HpBar hpBar;

    // HPバーの位置
    private Vector3 Position{
        get{
            return transform.position 
            + new Vector3(UnitBase.ModelPos.x * UnitBase.direction, UnitBase.ModelPos.y + UnitBase.ModelSize.y, 0);
        }
    }


    private void Start()
    {
        // HPバーのインスタンスを生成
        Vector3 pos = Position;
        if(UnitBase.Owner == OwnerType.PLAYER)
        {
            hpBarObj = Instantiate(hpBarPrefabAllay, pos, Quaternion.identity);
        }
        else
        {
            hpBarObj = Instantiate(hpBarParentEnemy, pos, Quaternion.identity);
        }
        hpBarObj.transform.SetParent(transform); // 子にする

        // HPバーの初期化
        hpBar = hpBarObj.GetComponent<HpBar>();
        hpBar.Initialize(UnitBase.MaxHP, UnitBase.ModelSize.x, height);

        // ダメージイベントに登録
        UnitBase.Events.AddOnDamageReceivedListener(OnDamageRecieved);
        UnitBase.Events.AddOnHealListener(OnHeal);
    }

    private void OnDamageRecieved(DamageInfo damageInfo)
    {
        // 被ダメージ時更新
        hpBar.UpdateHpBar(UnitBase.HP);
    }

    private void OnHeal(float obj)
    {
        // 回復時更新
        hpBar.UpdateHpBar(UnitBase.HP);
    }

    void OnDrawGizmosSelected()
    {
        // デバッグ用位置表示
        Gizmos.color = Color.red;
        Vector3 center = Position;
        Gizmos.DrawWireCube(center, new Vector3(UnitBase.ModelSize.x, height, 0f));
    }

}
