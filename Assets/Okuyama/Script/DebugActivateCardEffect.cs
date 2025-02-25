using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DebugActivateCardEffect : MonoBehaviour
{
    [Serializable]
    public struct ListObj
    {
        [SerializeField] public CardEffectBase cardEffect;
        [SerializeField] public bool button;
    }
    [SerializeField] private List<ListObj> player;
    [SerializeField] private List<ListObj> npc;

    private List<CardEffectBase> cardEffects;

    void Start()
    {
        // CardEffectBaseの派生クラスをのScriptableObjectを全て取得
        cardEffects = AssetDatabase.FindAssets("t:CardEffectBase")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<CardEffectBase>)
            .ToList();

        // Initialize player and npc lists
        player = new List<ListObj>();
        npc = new List<ListObj>();

        // Populate the lists with the found card effects
        foreach (var cardEffect in cardEffects)
        {
            player.Add(new ListObj { cardEffect = cardEffect, button = false });
            npc.Add(new ListObj { cardEffect = cardEffect, button = false });
        }
    }

    void Update()
    {
        for (int i = 0; i < player.Count; i++)
        {
            if (player[i].button)
            {
                player[i].cardEffect.Activate(OwnerType.PLAYER);
                var temp = player[i];
                temp.button = false;
                player[i] = temp;
            }
        }

        for(int i = 0; i < npc.Count; i++)
        {
            if (npc[i].button)
            {
                npc[i].cardEffect.Activate(OwnerType.NPC);
                var temp = npc[i];
                temp.button = false;
                npc[i] = temp;
            }
        }
    }

}
