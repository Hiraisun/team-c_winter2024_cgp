using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 案1 
/// 時間経過でマナ回復
/// マナ消費によってカードをドロー
/// </summary>
public class ResourceTest1 : PlayerResourceManager
{
    [SerializeField, Tooltip("初期マナ")]
    private float initialMana = 5;

    [SerializeField, Tooltip("1秒あたりのマナ回復量")]
    private float manaRegenPerSec = 1;

    [SerializeField, Tooltip("カードをドローするためのマナコスト")]
    private float drawCost = 1;

    [SerializeField, Tooltip("初期手札")]
    private int initialHandSize = 3;


    private void Start()
    {
        mana = initialMana;
        OnManaChanged?.Invoke();

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
    }

    public void UseManaToDraw()
    {
        if (mana < drawCost)
        {
            Debug.LogWarning("UseManaToDraw: マナが足りません。");
            return;
        }

        if(cardManager.DrawCard())
        {
            ConsumeMana(drawCost);
        }
    }
}
