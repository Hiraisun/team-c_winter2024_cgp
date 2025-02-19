using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // ��D�̃J�[�h���i�[����Ă��郊�X�g
    private List<Card> cards = new();

    // �g�p���邷�ׂẴJ�[�h
    private List<Card> deck = new();

    [SerializeField, Header("�ŏ��Ƀh���[����J�[�h�̖���")]
    int initialDrawCount = 5;

    [SerializeField, Header("�P���̃J�[�h�ɏ�����Ă���V���{���̐�")]
    int symbolCountPerCard = 4;

    [SerializeField, Header("�V���{���̃f�[�^���i�[���郊�X�g")]
    private List<SymbolData> allSymbols;

    // �I�𒆂̃J�[�h�̖���
    public int selectedCardCount = 0;

    private void Start()
    {

    }

    /// <summary>
    /// �f�b�L�̏�����
    /// </summary>
    private void InitializeDeck()
    {
        int n = symbolCountPerCard - 1;

        List<List<int>> cardLists = GenerateDobbleCards(n);

        deck.Clear(); // �f�b�L���N���A

        foreach (var symbols in cardLists)
        {
            deck.Add(CreateCard(symbols));
        }
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
    /// �����ɑΉ�����V���{�����J�[�h�ɑΉ�������
    /// </summary>
    /// <param name="_symbolIndices">�t�^����V���{���̃C���f�b�N�X</param>
    /// <returns></returns>
    private Card CreateCard(List<int> _symbolIndices)
    {
        Card card = new();
        card.symbols = new List<SymbolData>();

        foreach(int i in _symbolIndices)
        {
            card.symbols.Add(allSymbols[i]);
        }

        return card;
    }
}
