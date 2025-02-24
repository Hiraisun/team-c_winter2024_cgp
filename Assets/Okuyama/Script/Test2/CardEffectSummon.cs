

using UnityEngine;

/// <summary>
/// ユニット1体を召喚するカード効果
/// </summary>
[CreateAssetMenu(fileName = "Summon", menuName = "CardEffects/Summon")]
public class CardEffectSummon : CardEffectBase
{
    [SerializeField] GameObject unitPrefab; // 召喚するユニット

    protected override void Effect(OwnerType owner)
    {
        battleManager.SummonUnit(unitPrefab, owner);
    }
}
