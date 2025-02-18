using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基本的な移動処理
/// </summary>
[RequireComponent(typeof(UnitBase))] // UnitBase必須やで
public class UnitMoveNormal : MonoBehaviour
{
    [SerializeField] private UnitBase unitBase;
    [SerializeField] private float speed = 1.0f;

    void OnValidate()
    {
        // 自動アタッチ
        if(!Application.isPlaying)
        {
            unitBase = GetComponent<UnitBase>();
        }
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
