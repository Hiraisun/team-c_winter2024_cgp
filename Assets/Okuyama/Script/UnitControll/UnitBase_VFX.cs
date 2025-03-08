using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public partial class UnitBase{


    [SerializeField]
    private UnitVFXData unitVFXData;

    public void PlayAttackBuffVFX()
    {
        Instantiate(unitVFXData.AttackBuffPrefab, transform.position, Quaternion.identity);
    }

}



