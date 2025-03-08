using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

// どのユニットでも使うようなVFX関係
public partial class UnitBase{

    public void PlayAttackBuffVFX()
    {
        Debug.Log("攻撃バフエフェクト再生");
        Instantiate(unitVFXData.AttackBuffPrefab, ModelCenterPos, Quaternion.identity, ModelObject.transform);
    }
}



