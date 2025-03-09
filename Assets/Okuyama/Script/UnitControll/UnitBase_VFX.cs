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

    // 攻撃アップ
    private void PlayAttackBuffVFX()
    {
        Instantiate(unitVFXData.AttackBuffPrefab, ModelGlobalCenterPos, Quaternion.identity, ModelObject.transform);
    }

    // 隠密
    private void StartHideVFX()
    {
        ModelSpriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_HideOpacity", 0.5f);
        ModelSpriteRenderer.SetPropertyBlock(propertyBlock);
        // TODO:煙みたいな演出?
    }
    private void EndHideVFX()
    {
        ModelSpriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_HideOpacity", 1f);
        ModelSpriteRenderer.SetPropertyBlock(propertyBlock);
    }
}



