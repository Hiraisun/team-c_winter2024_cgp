using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Card : MonoBehaviour
{
    // CardManagerがSymbolを決定
    [HideInInspector]
    public List<SymbolData> symbols = new List<SymbolData>();

    [SerializeField, Header("シンボルを表示するImage")]
    private Image[] symbolImages;

    private CardManager cm;

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
            symbolImages[i].sprite = symbols[i].symbolSprite;
        }
    }

    // このオブジェクトがクリックされたとき
    private void OnMouseDown()
    {
        cm.selectedCardCount++;
    }
}




