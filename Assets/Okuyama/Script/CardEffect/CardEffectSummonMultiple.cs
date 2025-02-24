

using UnityEngine;
using System.Threading.Tasks;

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
        TriggerEffect(owner);
    }

    // scriptableObjectのためコルーチン使えない。
    // 代わりにTask.Delayを使って非同期処理を行う
    private async void TriggerEffect(OwnerType owner)
    {
        for (int i = 0; i < summonCount; i++)
        {
            battleManager.SummonUnit(unitPrefab, owner);
            await Task.Delay((int)(interval * 1000));  //TODO: TimeScale非対応。UniTasks検討
        }
    }
}
