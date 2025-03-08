using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffAttack", menuName = "CardEffects/BuffAttack")]
public class CardEffectBuffAttack : CardEffectBuffBase
{
    [Header("召喚時,攻撃力を倍率で増加させるバフ")]
    [SerializeField, Tooltip("増加倍率")]
    float buffRate = 2f;

    public override void Buff(GameObject unitObject, UnitBase unitBase)
    {
        // 召喚時に攻撃バフ
        unitBase.Events.AddOnSummonListener(() =>
        {
            unitBase.AttackMultiple(buffRate);
            return UniTask.CompletedTask;
        });
    }
}
