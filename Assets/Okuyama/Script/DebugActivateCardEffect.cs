using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugActivateCardEffect : MonoBehaviour
{
    [SerializeField] private CardEffectBase cardEffect;
    [SerializeField, Header("↓ 効果発動ボタン ↓")] 
    private bool button = false;

    void Update()
    {
        if(button)
        {
            cardEffect.Activate(OwnerType.PLAYER);
            button = false;
        }
    }
}
