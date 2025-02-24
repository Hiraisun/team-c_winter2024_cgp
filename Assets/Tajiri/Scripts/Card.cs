using UnityEngine;
using System;
public class Card : MonoBehaviour
{
    [SerializeField, Header("�J�[�h�ԍ�")]
    private int cardNum;

    public string actionDiscription;

    [SerializeField, Header("�V���{����\������SpriteRenderer")]
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
            int symbolIndex = cm.Deck[cardNum][i];
            sr[i].sprite = cm.AllSymbols[symbolIndex].symbolSprite;
        }
    }

    public void SetCardNum(int newNum)
    {
        cardNum = newNum;
        ApplyChanges();
    }

    public int GetCardNum()
    {
        return cardNum;
    }

    public void TrashAnimation()
    {

    }

    // ���̃I�u�W�F�N�g���N���b�N���ꂽ�Ƃ�
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




