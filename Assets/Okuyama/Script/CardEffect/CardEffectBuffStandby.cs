using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffStandby", menuName = "CardEffects/BuffStandby")]
public class CardEffectBuffStandby : CardEffectBuffBase
{
    public override void Buff(GameObject unitObject, UnitBase unitBase)
    {
        // コンポーネント付与
        unitObject.AddComponent<UnitActionStandby>();
    }
}
