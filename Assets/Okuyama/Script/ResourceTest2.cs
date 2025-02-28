using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 案2
/// 時間経過で自動ドロー
/// カードを捨てることでマナ回復
/// </summary>
public class ResourceTest2 : PlayerResourceManager
{
    [SerializeField, Tooltip("初期マナ")]
    private float initialMana = 5;

    [SerializeField, Tooltip("ドロー間隔(秒)")]
    private float drawInterval = 2;

    [SerializeField, Tooltip("初期手札")]
    private int initialHandSize = 5;

    [SerializeField, Tooltip("トラッシュで得られるマナ")]
    private float manaPerTrash = 3;

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
        drawTimer += Time.deltaTime;
        if (drawTimer >= drawInterval)
        {
            cardManager.DrawCard(); //TODO: 手札チェック
            drawTimer = 0;
        }
    }

    public void TrashForMana()
    {
        cardManager.TrashSelectedCard();
        mana += manaPerTrash;
        if (mana > maxMana) mana = maxMana;
        OnManaChanged?.Invoke();
    }
}
