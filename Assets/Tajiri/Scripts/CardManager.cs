using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // �g�p���邷�ׂẴJ�[�h
    [SerializeField]
    private List<List<int>> deck;

    // ��D
    [SerializeField]
    private List<int> hand;

    // ��D�̃Q�[���I�u�W�F�N�g
    [SerializeField]
    private List<GameObject> cards;

    [SerializeField, Header("�J�[�h��Prefab")]
    private GameObject cardPrefab;

    [SerializeField, Header("�ŏ��Ƀh���[����J�[�h�̖���")]
    int initialDrawCount = 5;

    [SerializeField, Header("�P���̃J�[�h�ɏ�����Ă���V���{���̐�")]
    int symbolCountPerCard = 4;

    [SerializeField, Header("�V���{���̃f�[�^���i�[���郊�X�g")]
    private List<SymbolData> allSymbols = new();

    // �I�𒆂̃J�[�h
    [SerializeField]
    public List<Card> selectedCards;

    private void Start()
    {
        // �f�b�L��������(�f�b�L�𐶐�)
        InitializeDeck();

        for(int i = 0; i <= initialDrawCount - 1; i++)
        {
            GameObject card = Instantiate(cardPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            cards.Add(card);
            hand.Add(-1);
            Draw(i);
        }
    }

    /// <summary>
    /// �f�b�L�̏�����
    /// </summary>
    private void InitializeDeck()
    {
        int n = symbolCountPerCard - 1;

        deck = GenerateDobbleCards(n);
    }

    /// <summary>
    /// Dobble�̃��[���ɉ������f�b�L���쐬����
    /// </summary>
    /// <param name="n">�J�[�h���̃V���{���̐�-1�̒l</param>
    /// <returns></returns>
    private List<List<int>> GenerateDobbleCards(int n)
    {
        List<List<int>> cards = new();

        // 1���ڂ̃J�[�h (0, 1, 2, ..., n)
        List<int> firstCard = new();
        for (int i = 0; i <= n; i++)
        {
            firstCard.Add(i);
        }
        cards.Add(firstCard);

        // ���� n �� (0 ���Œ肵�A�񂲂Ƃɑ��₵�Ă���)
        for (int i = 1; i <= n; i++)
        {
            List<int> card = new() { 0 };
            for (int j = 1; j <= n; j++)
            {
                card.Add(i + j * n);
            }
            cards.Add(card);
        }

        // �c��� n^2 �� (y = ax + b �̌`)
        for (int a = 1; a <= n; a++)
        {
            for (int b = 1; b <= n; b++)
            {
                List<int> card = new() { a };
                for (int x = 1; x <= n; x++)
                {
                    int symbol = (a * x + b) % n;
                    symbol = n + symbol * n + x;
                    card.Add(symbol);
                }
                cards.Add(card);
            }
        }

        return cards;
    }

    /// <summary>
    /// �J�[�h���I�����ꂽ�Ƃ��Ɏ��s�����X�N���v�g. �Q���I�΂ꂽ��Ԃ̎��ɍ�p����.
    /// </summary>
    public void UseCard()
    {
        // �I�����ꂽ�J�[�h��2���ȉ��Ȃ���s���Ȃ�
        if (selectedCards.Count != 2) return;

        // �I�����ꂽ�J�[�h�̃V���{���̃C���f�b�N�X�̃��X�g
        List<int> combinedSymbolIndices = new();

        // �������ꂽ���X�g����Q��o�ꂷ��V���{�������
        foreach(Card card in selectedCards)
        {
            card.isSelected = false;
            foreach(int index in card.symbolIndices)
            {
                // index�����łɃ��X�g�Ɋ܂܂�Ă����珈���𒆒f
                if (combinedSymbolIndices.Contains(index))
                {
                    // �f�o�b�O�p
                    Debug.Log(allSymbols[index].symbolSprite.name + "���Ăяo����܂����I");

                    // index�ɑΉ�����V���{���ɑΉ�����A�N�V���������s
                    //allSymbols[index].action.Execute();

                    // ���[�v���I��
                    break;
                }
                // index�����߂ďo�Ă������̂Ȃ烊�X�g�ɒǉ�
                combinedSymbolIndices.Add(index);
            }
        }

        // 2���̃J�[�h�̃g���b�V��
        List<int> selectedCardIndices = Trash();

        // 2���h���[
        Draw(selectedCardIndices[0]);
        Draw(selectedCardIndices[1]);
    }

    /// <summary>
    /// �I�����ꂽ�J�[�h���L�^���g���b�V������
    /// </summary>
    List<int> Trash()
    {
        List<int> cardsToRemove = new();
        List<int> selectedCardIndices = new();

        foreach (var _card in selectedCards)
        {
            cardsToRemove.Add(_card.cardNum);
        }

        foreach (var card in cardsToRemove)
        {
            selectedCardIndices.Add(hand.IndexOf(card));
            hand.Remove(card);
        }

        selectedCards.Clear();

        return selectedCardIndices;
    }


    /// <summary>
    /// �f�b�L����J�[�h��1���h���[����
    /// </summary>
    private void Draw(int index)
    {
        int randomIndex;

        while (true)
        {
            randomIndex = Random.Range(0, deck.Count);
            if (!hand.Contains(randomIndex)) break;
        }

        hand[index] = randomIndex;

        Card card = cards[index].GetComponent<Card>();
        card.symbolIndices = deck[hand[index]];
        card.cardNum = hand[index];
        card.symbols.Clear();

        for (int j = 0; j <= deck[hand[index]].Count - 1; j++)
        {
            card.symbols.Add(allSymbols[deck[hand[index]][j]]);
        }
        card.ApplySymbols();
    }
}
