using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// ���[�U�[����̏��
/// �����I�Ƀ|�[�Y���A�J�[�h���ʂ̊m�F���Ȃǒǉ����Ă���
/// </summary>
public enum ControllState{
    None,
    Selected,  //�J�[�h��1���I��
}

public class CardManager : MonoBehaviour
{
    // SerializeField-----------------------------------------

    [SerializeField, Header("�J�[�h��Prefab")]
    private GameObject cardPrefab;

    [SerializeField, Header("�V���{���f�[�^���i�[���郊�X�g")]
    private List<SymbolData> allSymbols = new();

    [SerializeField, Header("�ŏ��Ƀh���[����J�[�h�̖���")]
    int initialDrawCount = 5;

    [SerializeField, Header("�P���̃J�[�h�ɏ�����Ă���V���{���̐�")]
    int symbolCountPerCard = 4;

    // �v���p�e�B----------------------------------------------

    // ���[�U�[����̏��
    private ControllState controllState = ControllState.None;

    // �g�p���邷�ׂẴJ�[�h
    private List<List<int>> deck;

    // ��D
    private List<Card> hand = new();

    // �I�𒆂̃J�[�h 
    public Card selectedCard; // ���X�g����Ȃ�����

    // �����\���̂���J�[�hID (������Ǝ���������)
    private List<int> drawabkeCardIDs;


    private void Start()
    {
        // �f�b�L��������(�f�b�L�𐶐�)
        InitializeDeck();

        // DrawableCardIDs��������
        drawabkeCardIDs = Enumerable.Range(0, deck.Count).ToList();

        for(int i = 0; i < initialDrawCount; i++)
        {
            Draw();
        }
    }

    /// <summary>
    /// �f�b�L�̏�����
    /// </summary>
    private void InitializeDeck()
    {
        deck = CardAlgrithms.GenerateDobbleCards(symbolCountPerCard);
    }
    

    /// <summary>
    /// �f�b�L����J�[�h��1���h���[����
    /// </summary>
    public void Draw()
    {
        // TODO:��D�������ς��Ƃ��̏���
        

        // �����J�[�hID������
        CardAlgrithms.Shuffle(drawabkeCardIDs); //�V���b�t��
        int randomIndex = drawabkeCardIDs.First(); //�擪�����o��
        drawabkeCardIDs.RemoveAt(0);
        
        // �C���X�^���X��, ������
        GameObject cardObj = Instantiate(cardPrefab, this.transform); //�U�炩��̂ňꉞchild��
        Card card = cardObj.GetComponent<Card>();
        card.Initialize(randomIndex, this);

        // ��D�ɒǉ�
        hand.Add(card);
        card.ToHand();
        AdjustHandPosition(); //�ʒu����
    }


    /// <summary>
    /// �N���b�N���ꂽ���Ƃ�Card�ɒʒm���Ă��炤
    /// �������S���w���������Ă���B����������ƕ����������B�l�X�g�[�����N�\��Y
    /// </summary>
    public void NotifyCardClicked(Card card)
    {
        if (controllState == ControllState.None) //�����I�����Ă��Ȃ����
        {
            if (selectedCard != null) throw new Exception("State:None ���� selectedCard�����݂���");

            if (card.cardState == CardState.Hand) //��D���N���b�N����
            {
                SelectCard(card); //�I����Ԃ�
            }
        }
        else if (controllState == ControllState.Selected)  //1���I��
        {
            if(selectedCard == null) throw new Exception("State:Selected ���� selectedCard��null");

            if (card.cardState == CardState.Hand)  //��D���N���b�N����
            {
                if(TryUseCard(selectedCard, card)) //�J�[�h���g����
                {
                    selectedCard = null;
                    controllState = ControllState.None;
                }
                else //�J�[�h���g���Ȃ����� (�}�i�s���Ƃ�?)
                {
                    //TODO:�u�}�i�s���ł��v�I�ȃ��b�Z�[�W�Ƃ����Ƃ�
                }
            }
            else if (card.cardState == CardState.Selected) //�I�𒆂̃J�[�h���N���b�N����
            {
                DeselectCard(); //�I������
            }
        }

    }


    /// <summary>
    /// �S�Ă̎�D���ʒu��������
    /// </summary>
    private void AdjustHandPosition()
    {
        List<Vector3> handPositions = CardAlgrithms.CalculateHandPosition(hand.Count);
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].MoveToHandPos(handPositions[i]);
        }
    }


    // ControlState�n--------------------------------------------
    /// <summary>
    /// �J�[�h��I��
    /// </summary>
    private void SelectCard(Card card)
    {
        card.Select();
        selectedCard = card;
        controllState = ControllState.Selected;
    }
    /// <summary>
    /// �J�[�h��I������
    /// </summary>
    private void DeselectCard()
    {
        selectedCard.Deselect();
        selectedCard = null;
        controllState = ControllState.None;
    }
    /// <summary>
    /// �J�[�h��񖇎w�肵�Ďg�p(������)
    /// </summary>
    /// <returns>�J�[�h���g������true</returns>
    private bool TryUseCard(Card card1, Card card2)
    {

        // ��v�V���{����T��
        int matchSymbol = CardAlgrithms.FindMatchSymbol(card1, card2);

        // TODO:�}�i������Ȃ��Ƃ��Ŏg���Ȃ��Ȃ�false��Ԃ�

        // �V���{���ɑΉ�����A�N�V���������s
        //allSymbols[matchSymbol].action.Execute();
        Debug.Log(allSymbols[matchSymbol].symbolSprite.name + "���Ăяo����܂����I"); //�f�o�b�O�p

        // �I�����Z�b�g
        controllState = ControllState.None;
        selectedCard = null;

        // ������悤�ɂ���
        drawabkeCardIDs.Add(card1.cardNum);
        drawabkeCardIDs.Add(card2.cardNum);

        // ��D����폜
        hand.Remove(card1);
        hand.Remove(card2);

        // �g�p��ʒm -> �j��
        card1.Use();
        card2.Use();

        // �ʒu����
        AdjustHandPosition();
        return true;
    }



    // �����ŋ��ɂ�Getters----------------------------------------------------
    /// <summary>
    /// �J�[�hID�ɑΉ�����V���{�����擾
    /// </summary>
    public List<int> GetSymbols(int cardNum){
        // �������`�F�b�N
        if (cardNum < 0 || cardNum >= deck.Count) {
            Debug.LogError("Invalid card number: " + cardNum);
            return null;
        }
        return deck[cardNum];
    }

    /// <summary>
    /// �V���{��ID�ɑΉ�����SymbolData���擾
    /// </summary>
    public SymbolData GetSymbolData(int symbolIndex){
        // �������`�F�b�N
        if (symbolIndex < 0 || symbolIndex >= allSymbols.Count) {
            Debug.LogError("Invalid symbol index: " + symbolIndex);
            return null;
        }
        return allSymbols[symbolIndex];
    }


}
