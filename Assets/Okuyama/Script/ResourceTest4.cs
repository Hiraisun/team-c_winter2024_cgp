using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;



/// <summary>
/// 案4
/// マナ自動回復
/// 二回使用後、手札回復
/// </summary>
public class ResourceTest4 : PlayerResourceManager
{
    [SerializeField, Tooltip("初期マナ")]
    private float initialMana = 5;

    [SerializeField, Tooltip("毎秒のマナ回復量")]
    private float manaRegenPerSec = 1;

    [SerializeField, Tooltip("初期手札")]
    private int initialHandSize = 5;

    [SerializeField, Tooltip("手札回復までの使用回数")]
    private int handRecoveryCount = 2;

    private float useCount = 0;

    private void Start()
    {
        cardManager.AddCardUsedListener(HandleCardUsed);

        // 初期マナ
        mana = initialMana;
        OnManaChanged?.Invoke();

        // 初期手札
        DrawMultipleCard(initialHandSize).Forget();
    }

    private void Update()
    {
        // マナ回復
        mana += manaRegenPerSec * Time.deltaTime;
        if (mana > maxMana) mana = maxMana;
        OnManaChanged?.Invoke();
        
    }


    private void HandleCardUsed(){
        useCount++;
        if(useCount >= handRecoveryCount){
            useCount = 0;
            DrawMultipleCard(handRecoveryCount*2).Forget();
        }
    }


    private async UniTask DrawMultipleCard(int count){
        for (int i = 0; i < count; i++)
        {
            cardManager.DrawCard();
            await UniTask.Delay(200);
        }
    }

}
