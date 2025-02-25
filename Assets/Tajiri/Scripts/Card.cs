using UnityEngine;
using System;
using DG.Tweening;
using System.Linq;
using System.Collections.Generic;
public class Card : MonoBehaviour
{
    public int CardNum { get; private set; }

    public string EffectDiscription { get; private set; }

    public bool IsCardInHand { get; private set; }

    [SerializeField, Header("シンボルを表示するSpriteRenderer")]
    private SpriteRenderer[] sr;

    public event Action<Card> OnCardClicked;

    public void ApplyChanges(List<List<int>> deck, SymbolData[] allSymbols)
    {
        for(int i = 0; i < sr.Length; i++)
        {
            int symbolIndex = deck[CardNum][i];
            sr[i].sprite = allSymbols[symbolIndex].symbolSprite;
        }
    }

    public void SetHighlightedSymbol(List<List<int>> deck, List<int> matchingSymbols)
    {
        int highlightedSymbol = deck[CardNum].Intersect(matchingSymbols).DefaultIfEmpty(-1).First();
        int highlightedSymbolIndex = deck[CardNum].IndexOf(highlightedSymbol);

        if (highlightedSymbolIndex == -1) return;
        sr[highlightedSymbolIndex].DOKill();
        sr[highlightedSymbolIndex].DOColor(Color.green, 1f);
    }

    public void ResetSymbolsColor()
    {
        foreach(SpriteRenderer _sr in sr)
        {
            _sr.DOKill();
            _sr.DOColor(Color.white, 1f);
        }
    }

    public void SetCardNum(int value) => CardNum = value;

    public void SetCardInHand(bool value) => IsCardInHand = value;

    public void SetCardDescription(string value) => EffectDiscription = value;

    // このオブジェクトがクリックされたとき
    private void OnMouseDown()
    {
        OnCardClicked?.Invoke(this);
    }

    private void OnMouseEnter()
    {
        this.transform.DOScale(Vector2.one * 1.1f, 0.2f);
    }

    private void OnMouseExit()
    {
        this.transform.DOScale(Vector2.one, 0.2f);
    }
}




