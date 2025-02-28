#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class DebugActivateCardEffect : MonoBehaviour
{
    

    public List<CardEffectBase> allCardEffects;

    void Reset()
    {
        // CardEffectBaseの派生クラスをのScriptableObjectを全て取得
        allCardEffects = AssetDatabase.FindAssets("t:CardEffectBase")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<CardEffectBase>)
            .ToList();
    }
}

// カスタムエディタ
[CustomEditor(typeof(DebugActivateCardEffect))]
public class DebugActivateCardEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        DebugActivateCardEffect script = (DebugActivateCardEffect)target;

        GUILayout.Label("Player");
        foreach (var cardEffect in script.allCardEffects)
        {
            if(GUILayout.Button(cardEffect.name))
            {
                if (Application.isPlaying) cardEffect.Activate(OwnerType.PLAYER);
            }
        }

        GUILayout.Space(20);

        GUILayout.Label("NPC");
        foreach (var cardEffect in script.allCardEffects)
        {
            if(GUILayout.Button(cardEffect.name))
            {
                if (Application.isPlaying) cardEffect.Activate(OwnerType.NPC);
            }
        }
    }
}



#endif