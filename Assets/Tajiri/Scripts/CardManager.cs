using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // �g�p���邷�ׂẴJ�[�h
    private List<List<int>> deck;

    public List<List<int>> Deck
    {
        get { return deck; }
        private set { deck = value; }
    }
    private int[] handNums => cardCmps.Select(card => card.GetCardNum()).ToArray();

    // ��D�̃Q�[���I�u�W�F�N�g
    private GameObject[] cardObjs;

    private Card[] cardCmps;

    [SerializeField, Header("�J�[�h��Prefab")]
    private GameObject cardPrefab;

    [SerializeField, Header("��D�̖���")]
    private int MAX_HAND_CARDS = 5;

    [SerializeField, Header("�P���̃J�[�h�ɏ�����Ă���V���{���̐�")]
    private int SYMBOL_COUNT_PER_CARD = 4;

    [SerializeField, Header("�V���{���̃f�[�^���i�[����")]
    private SymbolData[] allSymbols;

    public SymbolData[] AllSymbols
    {
        get { return allSymbols; }
        private set { allSymbols = value; }
    }

    private void OnEnable()
    {
        cardObjs = new GameObject[MAX_HAND_CARDS];
        cardCmps = new Card[MAX_HAND_CARDS];

        deck = GenerateDobbleCards(SYMBOL_COUNT_PER_CARD);

        GenerateHandCards();
    }

    private List<List<int>> GenerateDobbleCards(int n)
    {
        n--;

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

    private void GenerateHandCards()
    {
        for (int i = 0; i <= MAX_HAND_CARDS - 1; i++)
        {
            cardObjs[i] = Instantiate(cardPrefab, new(-7 + 3 * i + 1, -3, 0), Quaternion.identity); //�z�u�Ɋւ��Ă͈ꎞ�I�ł�

            cardCmps[i] = cardObjs[i].GetComponent<Card>();
            cardCmps[i].Initialize(this);
            cardCmps[i].OnCardClicked += HandleCardClicked;
        }

        for (int i = 0; i <= MAX_HAND_CARDS - 1; i++)
        {
            TrashAndDraw(cardCmps[i]);
        }
    }

    private void TrashAndDraw(Card card)
    {
        var availableNums = Enumerable.Range(0, deck.Count).Except(handNums).ToList();

        int newCardNum = availableNums[Random.Range(0, availableNums.Count)];

        card.SetCardNum(newCardNum);
    }

    private Card selectedCard = null;

    private Dictionary<int, SymbolData> commonSymbol;

    private void HandleCardClicked(Card card)
    {
        if (selectedCard == null)
        {
            selectedCard = card;

            HashSet<int> candidateOfSymbols = new(deck[selectedCard.GetCardNum()]);

            commonSymbol = new();

            foreach (int handNum in handNums)
            {
                HashSet<int> handSymbols = new(deck[handNum]);

                HashSet<int> matchingSymbols = new(candidateOfSymbols.Intersect(handSymbols));

                foreach (int matchingSymbol in matchingSymbols)
                {
                    allSymbols[matchingSymbol].isHighlighted = true;
                    commonSymbol[handNum] = allSymbols[matchingSymbol];
                }
            }
        }

        else if (selectedCard != card)
        {
            //actionOfSymbols[card.cardNum].Execute();
            Debug.Log(commonSymbol[card.GetCardNum()].symbolSprite.name + "���Ăяo����܂���");

            TrashAndDraw(selectedCard);
            TrashAndDraw(card);

            selectedCard = null;
        }

        else
        {
            Debug.LogWarning("�����J�[�h�͑I���ł��܂���");
        }
    }

    
}
