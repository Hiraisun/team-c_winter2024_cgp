using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ユニットの頭上にHPバーを表示するコンポーネント
/// </summary>
public class UnitHpBar : UnitActionBase
{
    [Header("HPバー")]
    [SerializeField] private GameObject hpBarPrefabAllay; // HPバーのプレハブ
    [SerializeField] private GameObject hpBarParentEnemy; // 
    [SerializeField] private Vector3 position; // HPバーの位置調整用
    [SerializeField] private float width = 1f; // HPバーの幅
    [SerializeField] private float height = 0.05f; // HPバーの高さ

    private GameObject hpBarObj;
    private HpBar hpBar;


    private void Start()
    {
        // HPバーのインスタンスを生成
        Vector3 pos = transform.position + new Vector3(position.x * unitBase.direction, position.y, position.z);
        if(unitBase.Owner == OwnerType.PLAYER)
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
        hpBar.Initialize(unitBase.MaxHP, width, height);

        // ダメージイベントに登録
        unitBase.Events.AddOnDamageReceivedListener(OnDamageRecieved);
    }

    private void OnDamageRecieved(DamageInfo damageInfo)
    {
        // 被ダメージ時更新
        hpBar.UpdateHpBar(unitBase.HP);
    }

    void OnDrawGizmosSelected()
    {
        // デバッグ用位置表示
        Gizmos.color = Color.red;
        Vector3 center = transform.position + new Vector3(position.x * unitBase.direction, position.y, position.z);
        Gizmos.DrawWireCube(center, new Vector3(width, height, 0f));
    }

}
