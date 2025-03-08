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
    //[HideInInspector]
    [SerializeField, Tooltip("このユニットのUnitBase Resetで自動入力")] 
    private UnitBase unitBase;
    protected UnitBase UnitBase
    {
        get
        {
            if(unitBase == null)
            {
                unitBase = GetComponent<UnitBase>();
            }
            return unitBase;
        }
    }

    void Reset()
    {
        // UnitBaseの自動アタッチ
        unitBase = GetComponent<UnitBase>();
    }


    /// <summary>
    /// 行動が割り込まれた際の処理 行動終了ならばtrueを返す
    /// </summary>
    virtual public bool InterruptAction(){return false;}

}
