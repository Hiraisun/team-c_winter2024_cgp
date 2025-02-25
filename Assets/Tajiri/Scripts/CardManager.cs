using System;
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
    private int[] HandNums => cardCmps.Select(card => card.CardNum).ToArray();

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

        GenerateDobbleCards();

        GenerateHandCards();
    }

    private void GenerateDobbleCards()
    {
        int n = SYMBOL_COUNT_PER_CARD - 1;

        deck = new();

        // 1���ڂ̃J�[�h (0, 1, 2, ..., n)
        List<int> firstCard = new();
        for (int i = 0; i <= n; i++)
        {
            firstCard.Add(i);
        }
        deck.Add(firstCard);

        // ���� n �� (0 ���Œ肵�A�񂲂Ƃɑ��₵�Ă���)
        for (int i = 1; i <= n; i++)
        {
            List<int> card = new() { 0 };
            for (int j = 1; j <= n; j++)
            {
                card.Add(i + j * n);
            }
            deck.Add(card);
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
                deck.Add(card);
            }
        }
    }

    private void GenerateHandCards()
    {
        for (int i = 0; i < MAX_HAND_CARDS; i++)
        {
            cardObjs[i] = Instantiate(cardPrefab, new(-7 + 3 * i + 1, -3, 0), Quaternion.identity); //�z�u�Ɋւ��Ă͈ꎞ�I�ł�

            cardCmps[i] = cardObjs[i].GetComponent<Card>();
            cardCmps[i].Initialize(this);
            cardCmps[i].OnCardClicked += HandleCardClicked;
        }

        for (int i = 0; i < MAX_HAND_CARDS; i++)
        {
            TrashAndDraw(cardCmps[i]);
        }
    }

    private void TrashAndDraw(Card card)
    {
        var availableNums = Enumerable.Range(0, deck.Count).Except(HandNums).ToList();

        int newCardNum = availableNums[UnityEngine.Random.Range(0, availableNums.Count)];

        card.SetCardNum(newCardNum);
        card.ApplyChanges();
    }

    private Card selectedCard = null;

    private HashSet<int> matchingSymbols;

    public List<int> MatchingSymbols
    {
        get { return matchingSymbols.ToList(); }
        private set { matchingSymbols = value.ToHashSet(); }
    }

    private Dictionary<int, SymbolData> commonSymbol;

    private void HandleCardClicked(Card card)
    {
        if (selectedCard == null)
        {
            selectedCard = card;

            HashSet<int> candidateOfSymbols = new(deck[selectedCard.CardNum]);

            commonSymbol = new();

            foreach (int handNum in HandNums)
            {
                HashSet<int> handSymbols = new(deck[handNum]);

                matchingSymbols = new(candidateOfSymbols.Intersect(handSymbols));

                foreach (Card _card in cardCmps)
                {
                    if (_card != card)
                    {
                        _card.SetHighlightedSymbol();
                    }
                }

                foreach (int matchingSymbol in matchingSymbols)
                {
                    commonSymbol[handNum] = allSymbols[matchingSymbol];
                }
            }
        }

        else if (selectedCard != card)
        {
            if (commonSymbol[card.CardNum].action != null)
            {
                commonSymbol[card.CardNum].action.Execute();
            }
            else
            {
                Debug.LogWarning(commonSymbol[card.CardNum].symbolSprite.name + " �ɃA�N�V�������ݒ肳��Ă��܂���B");
            }
    
            TrashAndDraw(selectedCard);
            TrashAndDraw(card);

            selectedCard = null;

            foreach (Card _card in cardCmps)
            {
                _card.ResetSymbolsColor();
            }
        }

        else
        {
            Debug.LogWarning("�����J�[�h�͑I���ł��܂���");
        }
    }
}
