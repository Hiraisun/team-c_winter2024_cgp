using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// ユニットの行動処理コンポーネントの基底クラス
/// UnitBaseに依存する
/// </summary>
[RequireComponent(typeof(UnitBase))]
public abstract class UnitActionBase : MonoBehaviour
{
    [SerializeField] protected UnitBase unitBase;

    void Reset()
    {
        // UnitBaseの自動アタッチ
        unitBase = GetComponent<UnitBase>();
    }

    void Awake()
    {
        unitBase.RegisterOnDeath(OnDeath);
        unitBase.RegisterOnAttackStart(OnAttackStart);
        unitBase.RegisterOnDamageDealt(OnDamageDealt);
        unitBase.RegisterOnDamageReceived(OnDamageRecieved);
    }



    /// <summary>
    /// 死亡時の処理
    /// </summary>
    virtual protected void OnDeath(){}

    /// <summary>
    /// 攻撃開始時の処理
    /// </summary>
    virtual protected void OnAttackStart(){}

    /// <summary>
    /// 与ダメージ時の処理
    /// </summary>
    virtual protected void OnDamageDealt(UnitBase target){}

    /// <summary>
    /// 被ダメージ時の処理
    /// </summary>
    virtual protected void OnDamageRecieved(float damage){}
    
}
