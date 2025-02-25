using UnityEngine;
using System;
using DG.Tweening;
using System.Linq;
public class Card : MonoBehaviour
{
    public int CardNum { get; private set; }

    public string actionDiscription;

    [SerializeField, Header("シンボルを表示するSpriteRenderer")]
    private SpriteRenderer[] sr;

    public enum CardState
    {
        Idle,
        Selected,
        Trashing
    }

    public CardState currentState = CardState.Idle;

    public event Action<Card> OnCardClicked;

    private CardManager cm;

    public void Initialize(CardManager manager)
    {
        cm = manager;
    }

    public void ApplyChanges()
    {
        for(int i = 0; i < sr.Length; i++)
        {
            int symbolIndex = cm.Deck[CardNum][i];
            sr[i].sprite = cm.AllSymbols[symbolIndex].symbolSprite;
        }
    }

    public void SetHighlightedSymbol()
    {
        int highlightedSymbol = cm.Deck[CardNum].Intersect(cm.MatchingSymbols).DefaultIfEmpty(-1).First();
        int highlightedSymbolIndex = cm.Deck[CardNum].IndexOf(highlightedSymbol);

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

    public void SetCardNum(int newNum) => CardNum = newNum;

    public void TrashAnimation()
    {

    }

    // このオブジェクトがクリックされたとき
    private void OnMouseDown()
    {
        OnCardClicked?.Invoke(this);
    }

    private void OnMouseEnter()
    {
        transform.DOKill();
        this.transform.DOScale(Vector2.one * 1.1f, 0.2f);
    }

    private void OnMouseExit()
    {
        transform.DOKill();
        this.transform.DOScale(Vector2.one, 0.2f);
    }
}




