using UnityEngine;
using System;
public class Card : MonoBehaviour
{
    [Header("カード番号")]
    public int cardNum;

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
        // CardManagerを取得
        cm = GameObject.FindAnyObjectByType<CardManager>();
    }

    public void Initialize()
    {
        for(int i = 0; i <= sr.Length - 1; i++)
        {
            int symbolIndex = cm.Deck[cardNum][i];
            sr[i].sprite = cm.AllSymbols[symbolIndex].symbolSprite;
        }
    }

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




