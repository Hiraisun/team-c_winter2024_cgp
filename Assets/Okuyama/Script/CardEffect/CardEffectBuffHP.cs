using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffHP", menuName = "CardEffects/BuffHP")]
public class CardEffectBuffHP : CardEffectBuffBase
{
    [Header("召喚時,HPを倍率で増加させるバフ")]
    [SerializeField, Tooltip("増加倍率")]
    float buffRate = 2f;

    public override void Buff(GameObject unitObject, UnitBase unitBase)
    {
        // 召喚時にHPバフ
        unitBase.Events.AddOnSummonListener(() =>
        {
            unitBase.HPMultiple(buffRate);
            return UniTask.CompletedTask;
        });
    }
}
