using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基本的な移動処理
/// </summary>
public class UnitMoveNormal : UnitActionBase
{
    [SerializeField] private float speed = 1.0f;

    // 自動アタッチ
    void Reset()
    {
        unitBase = GetComponent<UnitBase>();
    }

    void Update()
    {
        // アクション中以外は移動
        if(!unitBase.isBusy)
        {
            transform.position -= new Vector3(speed * Time.deltaTime * unitBase.direction, 0, 0);
        }
    }
}
