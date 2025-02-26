

using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

/// <summary>
/// ユニット複数体を召喚するカード効果
/// </summary>
[CreateAssetMenu(fileName = "SummonMultiple", menuName = "CardEffects/SummonMultiple")]
public class CardEffectSummonMultiple : CardEffectBase
{
    [SerializeField, Tooltip("召喚するユニット")] 
    GameObject unitPrefab; 

    [SerializeField, Tooltip("ユニットの数")] 
    int summonCount; 
    
    [SerializeField, Tooltip("間隔(秒)")] 
    float interval; 

    protected override void Effect(OwnerType owner)
    {
        TriggerEffect(owner).Forget(); // TODO:効果中の演出つけるならawaitする
    }

    // 一定時間おきにユニットを召喚
    private async UniTask TriggerEffect(OwnerType owner)
    {
        for (int i = 0; i < summonCount; i++)
        {
            battleManager.SummonUnit(unitPrefab, owner);
            await UniTask.WaitForSeconds(interval);
        }
    }
}
