

using UnityEngine;

/// <summary>
/// ユニット1体を召喚するカード効果
/// </summary>
[CreateAssetMenu(fileName = "Summon", menuName = "CardEffects/Summon")]
public class CardEffectSummon : CardEffectBase
{
    [SerializeField, Tooltip("召喚するユニット")]
    GameObject unitPrefab;

    protected override void CardEffect(OwnerType owner)
    {
        battleManager.SummonUnit(unitPrefab, owner);
    }
}
