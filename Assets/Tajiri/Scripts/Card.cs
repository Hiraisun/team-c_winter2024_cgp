using UnityEngine;
using System;
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

    private void OnEnable()
    {

    }

    public void Initialize(CardManager manager)
    {
        cm = manager;
    }

    public void ApplyChanges()
    {
        for(int i = 0; i <= sr.Length - 1; i++)
        {
            int symbolIndex = cm.Deck[CardNum][i];
            sr[i].sprite = cm.AllSymbols[symbolIndex].symbolSprite;
        }
    }

    public void SetHighlightedSymbol(int[] symbolIndices)
    {

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
        
    }

    private void OnMouseExit()
    {

    }
}




