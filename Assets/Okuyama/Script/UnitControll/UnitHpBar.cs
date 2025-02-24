using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ユニットの頭上にHPバーを表示するコンポーネント
/// </summary>
public class UnitHpBar : UnitActionBase
{
    [SerializeField] private GameObject hpBarPrefab; // HPバーのプレハブ

    [Header("位置調整")]
    [SerializeField] private Vector3 pos; // HPバーの位置調整用
    [SerializeField] private float width = 1f; // HPバーの幅
    [SerializeField] private float height = 0.05f; // HPバーの高さ

    private GameObject hpBarObj;
    private HpBar hpBar;


    private void Start()
    {
        // HPバーのインスタンスを生成
        hpBarObj = Instantiate(hpBarPrefab, transform.position + pos, Quaternion.identity, transform);
        hpBar = hpBarObj.GetComponent<HpBar>();
        Debug.Log(width);
        hpBar.Initialize(unitBase.MaxHP, width, height);
    }

    protected override void OnDamageRecieved(float damage)
    {
        // 被ダメージ時更新
        hpBar.UpdateHealth(unitBase.HP);
    }


    void OnDrawGizmosSelected()
    {
        // デバッグ用位置表示
        Gizmos.color = Color.red;
        Vector3 center = transform.position + pos;
        Gizmos.DrawWireCube(center, new Vector3(width, height, 0f));
    }

}
