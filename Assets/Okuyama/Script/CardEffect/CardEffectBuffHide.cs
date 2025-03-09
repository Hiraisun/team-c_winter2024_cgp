using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffHide", menuName = "CardEffects/BuffHide")]
public class CardEffectBuffHide : CardEffectBuffBase
{
    public override void Buff(GameObject unitObject, UnitBase unitBase)
    {
        // 召喚完了時に隠密
        unitBase.Events.AddOnSummonCompleteListener(() =>
        {
            unitBase.Hide();
        });
    }
}
