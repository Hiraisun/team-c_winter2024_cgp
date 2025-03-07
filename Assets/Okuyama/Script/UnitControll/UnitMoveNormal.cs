using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基本的な移動処理
/// </summary>
public class UnitMoveNormal : UnitActionBase
{
    [Header("移動")]
    [SerializeField, Tooltip("移動速度")]
    private float speed = 1.0f;

    // 自動アタッチ
    void Reset()
    {
        unitBase = GetComponent<UnitBase>();
    }

    void Update()
    {
        if(unitBase.UnitState != UnitState.MAIN) return;

        if(unitBase.IsBusy) return;

        // アクション中でなければ移動
        transform.position -= new Vector3(speed * Time.deltaTime * unitBase.direction, 0, 0);
    }
}
