using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffDeathHeal", menuName = "CardEffects/BuffDeathHeal")]
public class CardEffectBuffDeathHeal : CardEffectBuffBase
{
    [SerializeField, Tooltip("回復量")]
    float healAmount = 3;

    public override void Buff(GameObject unitObject, UnitBase unitBase)
    {
        // コンポーネント付与
        UnitDeathHeal deathHeal = unitObject.AddComponent<UnitDeathHeal>();
        deathHeal.SetHealAmount(healAmount);
    }
}
