using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    // 使用するすべてのカード
    private List<List<int>> deck;

    public List<List<int>> Deck
    {
        get { return deck; }
        private set { deck = value; }
    }
    private int[] HandNums => cardCmps.Select(card => card.CardNum).ToArray();

    // 手札のゲームオブジェクト
    private GameObject[] cardObjs;

    private Card[] cardCmps;

    [SerializeField, Header("カードのPrefab")]
    private GameObject cardPrefab;

    [SerializeField, Header("１枚のカードに書かれているシンボルの数")]
    private int SYMBOL_PER_CARD = 4;

    private int TOTAL_CARD_AND_SYMBOL => SYMBOL_PER_CARD * SYMBOL_PER_CARD - SYMBOL_PER_CARD + 1; // カードの枚数とシンボルの数は同じ

    [SerializeField, Header("手札の枚数")]
    private int INITIAL_HAND_CARDS = 5;

    [SerializeField, Header("手札の最大枚数")]
    private int MAX_HAND_CARDS = 8;

    private SymbolData[] allSymbols;

    public SymbolData[] AllSymbols
    {
        get { return allSymbols; }
        private set { allSymbols = value; }
    }

    private void OnEnable()
    {
        InitializeArrays();

        GenerateDobbleCardsList();

        GenerateCardsObj();

        InitialDraw();
    }

    private void InitializeArrays()
    {
        allSymbols = new SymbolData[TOTAL_CARD_AND_SYMBOL];
        cardObjs = new GameObject[TOTAL_CARD_AND_SYMBOL];
        cardCmps = new Card[TOTAL_CARD_AND_SYMBOL];

        allSymbols = Resources.LoadAll<SymbolData>("Symbols");
    }

    private void GenerateDobbleCardsList()
    {
        int n = SYMBOL_PER_CARD - 1;

        deck = new();

        // 1枚目のカード (0, 1, 2, ..., n)
        List<int> firstCard = new();
        for (int i = 0; i <= n; i++)
        {
            firstCard.Add(i);
        }
        deck.Add(firstCard);

        // 次の n 枚 (0 を固定し、列ごとに増やしていく)
        for (int i = 1; i <= n; i++)
        {
            List<int> card = new() { 0 };
            for (int j = 1; j <= n; j++)
            {
                card.Add(i + j * n);
            }
            deck.Add(card);
        }

        // 残りの n^2 枚 (y = ax + b の形)
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

    private void GenerateCardsObj()
    {
        for (int i = 0; i < TOTAL_CARD_AND_SYMBOL; i++)
        {
            cardObjs[i] = Instantiate(cardPrefab, new(0, 10, 0), Quaternion.identity); //配置に関しては一時的です
            cardObjs[i].SetActive(false);

            cardCmps[i] = cardObjs[i].GetComponent<Card>();
            cardCmps[i].SetCardNum(i);
            cardCmps[i].Initialize(this);
            cardCmps[i].ApplyChanges();
            cardCmps[i].OnCardClicked += HandleCardClicked;
        }
    }

    private void InitialDraw()
    {
        for(int i = 0; i < INITIAL_HAND_CARDS; i++)
        {
            Draw();
        }
    }

    private void Trash(Card selectedCard)
    {
        if (selectedCard == null)
        {
            Debug.LogWarning("捨てるカードが選択されていません。");
            return;
        }

        selectedCard.transform.position = new Vector3(0, 10, 0);
        selectedCard.gameObject.SetActive(false);
        selectedCard = null;
        RearrangeHand();
    }

    private void Draw()
    {
        if (cardCmps.Count(c => c.gameObject.activeSelf) >= MAX_HAND_CARDS)
        {
            Debug.LogWarning("手札が最大枚数に達しています。");
            return;
        }

        var inactiveCards = cardCmps.Where(c => !c.gameObject.activeSelf).ToList();

        if (inactiveCards.Count > 0)
        {
            Card newCard = inactiveCards[UnityEngine.Random.Range(0, inactiveCards.Count)];
            newCard.gameObject.SetActive(true);
            RearrangeHand();
        }
        else
        {
            Debug.LogWarning("非アクティブなカードがありません。");
        }
    }

    private void RearrangeHand()
    {
        int index = 0;
        foreach (var card in cardCmps.Where(c => c.gameObject.activeSelf))
        {
            card.transform.DOMove(new Vector3(-6 + index * 3, 0, 0), 1f);
            index++;
        }
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
            try
            {
                commonSymbol[card.CardNum].cardAction.Activate(OwnerType.PLAYER);
            }
            catch
            {
                Debug.LogWarning(commonSymbol[card.CardNum].symbolSprite.name + " に効果が設定されていません。");
            }

            Trash(selectedCard);
            Trash(card);

            selectedCard = null;

            foreach (Card _card in cardCmps)
            {
                _card.ResetSymbolsColor();
            }
        }

        else
        {
            Debug.LogWarning("同じカードは選択できません");
        }
    }
}
