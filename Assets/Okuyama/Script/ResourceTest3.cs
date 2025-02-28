using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 案2
/// 手札、マナ 共に時間経過で回復
/// </summary>
public class ResourceTest3 : PlayerResourceManager
{
    [SerializeField, Tooltip("初期マナ")]
    private float initialMana = 5;

    [SerializeField, Tooltip("毎秒のマナ回復量")]
    private float manaRegenPerSec = 1;

    [SerializeField, Tooltip("初期手札")]
    private int initialHandSize = 3;

    [SerializeField, Tooltip("ドロー間隔(秒)")]
    private float drawInterval = 2;

    private float drawTimer = 0;

    private void Start()
    {
        // 初期マナ
        mana = initialMana;
        OnManaChanged?.Invoke();

        // 初期手札
        for (int i = 0; i < initialHandSize; i++)
        {
            cardManager.DrawCard();
        }
    }

    private void Update()
    {
        // マナ回復
        mana += manaRegenPerSec * Time.deltaTime;
        if (mana > maxMana) mana = maxMana;
        OnManaChanged?.Invoke();
        
        // 自動ドロー
        drawTimer += Time.deltaTime;
        if (drawTimer >= drawInterval)
        {
            if(cardManager.DrawCard())
            {
                drawTimer -= drawInterval;
            }
            else
            {
                // 引かないなら手札が空き次第
                drawTimer = drawInterval;
            }
        }
    }

}
