using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using System.Collections;

public class CardManager : MonoBehaviour
{
    // 使用するすべてのカード
    private List<List<int>> deck;

    // 手札のゲームオブジェクト
    private GameObject[] cardObjs;

    private Card[] cardCmps;

    [SerializeField, Header("カードのPrefab")]
    private GameObject cardPrefab;

    [SerializeField, Header("１枚のカードに書かれているシンボルの数")]
    private int SYMBOL_PER_CARD = 4;

    [SerializeField, Header("手札の枚数")]
    private int INITIAL_HAND_CARDS = 5;

    [SerializeField, Header("手札の最大枚数")]
    private int MAX_HAND_CARDS = 8;

    [SerializeField, Header("ドローのクールタイム")]
    private float DRAW_COOLTIME = 5f;

    [SerializeField, Header("手札の位置")]
    private Vector3 handPos;

    [SerializeField, Header("カード間の隙間"), Range(1, 5)]
    private float gapSizeMagnification = 3f;

    private float gapSize;

    private Vector3 trashPos = new(0, 10, 0);

    private SymbolData[] allSymbols;

    private int TOTAL_CARD_AND_SYMBOL => SYMBOL_PER_CARD * SYMBOL_PER_CARD - SYMBOL_PER_CARD + 1; // カードの枚数とシンボルの数は同じ

    private int[] HandNums => cardCmps.Select(card => card.CardNum).ToArray();

    private void OnEnable()
    {
        InitializeArrays();

        GenerateDobbleCardsList();

        GenerateCardsObj();

        InitialDraw();

        StartCoroutine(DrawLoop());
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
            cardObjs[i] = Instantiate(cardPrefab, trashPos, Quaternion.identity); //配置に関しては一時的です

            cardCmps[i] = cardObjs[i].GetComponent<Card>();
            cardCmps[i].SetCardInHand(false);
            cardCmps[i].SetCardNum(i);
            cardCmps[i].ApplyChanges(deck, allSymbols);
            cardCmps[i].OnCardClicked += HandleCardClicked;
        }
    }

    private void InitialDraw()
    {
        for (int i = 0; i < INITIAL_HAND_CARDS; i++)
        {
            Draw();
        }
    }

    IEnumerator DrawLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(DRAW_COOLTIME);
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

        selectedCard.SetCardInHand(false);
        selectedCard.transform.DOMove(trashPos, 1f);
        selectedCard.transform.DORotate(new Vector3(0 ,0 ,0), 1f);
        RearrangeHand();
    }

    public void Draw()
    {
        if (cardCmps.Count(c => c.IsCardInHand) >= MAX_HAND_CARDS)
        {
            Debug.LogWarning("手札が最大枚数に達しています。");
            return;
        }

        var inactiveCards = cardCmps.Where(c => !c.IsCardInHand).ToList();

        if (inactiveCards.Count > 0)
        {
            Card newCard = inactiveCards[UnityEngine.Random.Range(0, inactiveCards.Count)];
            newCard.SetCardInHand(true);
            RearrangeHand();
        }
        else
        {
            Debug.LogWarning("非アクティブなカードがありません。");
        }
    }

    private void RearrangeHand()
    {
        var cardsInHand = cardCmps.Where(c => c.IsCardInHand).ToList();
        var (positions, rotations) = GetCardTransforms(cardsInHand.Count, 5f, 45f);

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].transform.DOMove(positions[i], 1f);
            cardsInHand[i].transform.DORotateQuaternion(rotations[i], 1f);
        }
    }

    private (List<Vector3>, List<Quaternion>) GetCardTransforms(int cardCount, float radius, float maxAngle)
    {
        List<Vector3> positions = new();
        List<Quaternion> rotations = new();

        if (cardCount <= 0) return (positions, rotations);

        float startAngle = -maxAngle * 0.5f;
        float angleStep = (cardCount > 1) ? maxAngle / (cardCount - 1) : 0;
        Vector3 center = handPos - new Vector3(0, radius, 0);

        if (cardCount == 1)
        {
            Vector3 position = center + new Vector3(0 ,radius, 0);
            positions.Add(position);

            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            rotations.Add(rotation);

            return (positions, rotations);
        }

        gapSize = cardCount * gapSizeMagnification * 0.1f;

        for (int i = 0; i < cardCount; i++)
        {
            float angle = startAngle + angleStep * i;
            float radian = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(radian) * radius * gapSize;
            float y = Mathf.Cos(radian) * radius;

            Vector3 position = center + new Vector3(x, y, -i * 0.01f);
            positions.Add(position);

            Quaternion rotation = Quaternion.Euler(0, 0, -angle);
            rotations.Add(rotation);
        }

        return (positions, rotations);
    }

    private Card selectedCard = null;

    private HashSet<int> matchingSymbols;

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
                        _card.SetHighlightedSymbol(deck, matchingSymbols.ToList());
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

/// <summary>
/// エディターの設定
/// </summary>
#if UNITY_EDITOR
[CustomEditor(typeof(CardManager))]
public class CardManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CardManager t = target as CardManager;

        if (GUILayout.Button("ドロー"))
        {
            t.Draw();
        }
    }
}
#endif
