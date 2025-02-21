using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class Card : MonoBehaviour
{
    [Header("カード番号")]
    public int cardNum;
    // CardManagerがSymbolを決定
    [HideInInspector]
    public List<SymbolData> symbols;

    [SerializeField, Header("シンボルを表示するSpriteRenderer")]
    private SpriteRenderer[] sr;

    private CardManager cm;

    public List<int> symbolIndices = new();

    [HideInInspector]
    public bool isSelected = false;

    private void OnEnable()
    {
        // CardManagerを取得
        cm = GameObject.FindAnyObjectByType<CardManager>();
    }

    // SymbolのSpriteをImageに適用
    public void ApplySymbols()
    {
        for (int i = 0; i < symbols.Count; i++)
        {
            sr[i].sprite = symbols[i].symbolSprite;
        }
    }

    // このオブジェクトがクリックされたとき
    private void OnMouseDown()
    {
        if (isSelected) return;

        isSelected = true;

        cm.selectedCards.Add(this.GetComponent<Card>());
        cm.UseCard();
    }
}




