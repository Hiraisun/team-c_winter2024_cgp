using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffThorn", menuName = "CardEffects/BuffThorn")]
public class CardEffectBuffThorn : CardEffectBuffBase
{
    [SerializeField, Tooltip("付与するトゲのダメージ値")]
    float thornDamage = 3;

    public override void Buff(GameObject unitObject, UnitBase unitBase)
    {
        // コンポーネント付与
        UnitThorn unitThorn = unitObject.AddComponent<UnitThorn>();
        unitThorn.SetThornDamage(thornDamage);
    }
}
