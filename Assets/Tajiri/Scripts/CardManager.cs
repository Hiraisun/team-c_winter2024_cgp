using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR;

public class CardManager : MonoBehaviour
{
    // 使用するすべてのカード
    private List<List<int>> deck;

    // 全カードのゲームオブジェクト配列
    private GameObject[] cardObjs;

    // 全Cardインスタンスの配列
    private Card[] cardCmps;

    [SerializeField, Header("カードのPrefab")]
    private GameObject cardPrefab;

    [SerializeField, Header("１枚のカードに書かれているシンボルの数")]
    private int SYMBOL_PER_CARD = 4;

    [SerializeField, Header("手札の最大枚数")]
    private int MAX_HAND_CARDS = 8;

    [Space(10)]
    [SerializeField, Header("手札位置の調整パラメータ")]
    private CardAlgorithms.HandPosConfig handPosConfig = new()
    {
        handPos = new Vector3(0, 0, 0),
        handRadius = 5f,
        handMaxAngle = 45f,
        gapSizeMagnification = 3f
    };

    [SerializeField, Header("ドロー時出現位置")]
    private Vector3 drawPos = new(0, 10, 0); //初期位置として利用

    [SerializeField, Header("トラッシュ時移動先")]
    private Vector3 trashPos = new(0, 0, 0);

    private SymbolData[] allSymbols;

    private int TOTAL_CARD_AND_SYMBOL => SYMBOL_PER_CARD * SYMBOL_PER_CARD - SYMBOL_PER_CARD + 1; 
    // カードの枚数とシンボルの数は同じ

    private int[] HandNums => cardCmps.Select(card => card.CardNum).ToArray();

    // 選択中のカード
    private Card selectedCard = null;

    // 初期化処理
    private void Awake()
    {
        InitializeArrays();
        deck = CardAlgorithms.GenerateDobbleCardsList(SYMBOL_PER_CARD);
        GenerateCardsObj();
    }

    // メモリ確保
    private void InitializeArrays()
    {
        allSymbols = new SymbolData[TOTAL_CARD_AND_SYMBOL];
        cardObjs = new GameObject[TOTAL_CARD_AND_SYMBOL];
        cardCmps = new Card[TOTAL_CARD_AND_SYMBOL];

        allSymbols = Resources.LoadAll<SymbolData>("Symbols");
    }

    // カードオブジェクトの生成(初期化)
    private void GenerateCardsObj()
    {
        for (int i = 0; i < TOTAL_CARD_AND_SYMBOL; i++)
        {
            cardObjs[i] = Instantiate(cardPrefab, trashPos, Quaternion.identity, transform); //配置に関しては一時的です
            // Managerの子オブジェクトとした。シーンが散らかるので。

            // 参照取得処理
            cardCmps[i] = cardObjs[i].GetComponent<Card>();
            cardCmps[i].Initialize(i, deck, allSymbols);            // 初期化
            cardCmps[i].AddCardClickedListener(HandleCardClicked);  // クリックイベントを聴く
        }
    }

    /// <summary>
    /// カードを1枚引く
    /// </summary>
    public void Draw()
    {
        // 例外処理
        if (cardCmps.Count(c => c.IsCardInHand) >= MAX_HAND_CARDS)
        {
            Debug.LogWarning("手札が最大枚数に達しています。");
            return;
        }

        // 手札にないカード
        var inactiveCards = cardCmps.Where(c => !c.IsCardInHand).ToList();

        if (inactiveCards.Count > 0)
        {
            Card newCard = inactiveCards[UnityEngine.Random.Range(0, inactiveCards.Count)];
            newCard.ToHand(); // 手札に加える
            RearrangeHand();
        }
        else
        {
            Debug.LogWarning("非アクティブなカードがありません。");
        }
    }

    ///// <summary>
    ///// 指定したカードを捨てる   ->   「捨てる」「使う」は別で定義すべきかも
    ///// </summary>
    ///// <param name="selectedCard"></param>
    //private void Trash(Card selectedCard)
    //{
    //    if (selectedCard == null)
    //    {
    //        Debug.LogWarning("捨てるカードが選択されていません。");
    //        return;
    //    }
    //
    //    selectedCard.SetCardInHand(false);
    //    selectedCard.MoveTo(trashPos, Quaternion.Euler(0, 0, 0));
    //    RearrangeHand();
    //}


    /// <summary>
    /// 手札のカードを再配置する
    /// </summary>
    private void RearrangeHand()
    {
        var cardsInHand = cardCmps.Where(c => c.IsCardInHand).ToList();
        var (positions, rotations) = CardAlgorithms.CalculateHandPos(cardsInHand.Count, handPosConfig);

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].MoveTo(positions[i], rotations[i]);
        }
    }

    /// <summary>
    /// カードクリックイベントのハンドラメソッド。
    /// 
    /// !!! イベントハンドリング (クリック時の処理の振り分け) のみを行う !!!
    /// 気を抜くとここが肥大化する。分散しよう。
    /// 
    /// </summary>
    private void HandleCardClicked(Card card)
    {
        if (selectedCard == null) // 選択中でない 
        {
            selectedCard = card;

            HashSet<int> candidateOfSymbols = new(deck[selectedCard.CardNum]);

            Dictionary<int, SymbolData> commonSymbol = new();

            HashSet<int> matchingSymbols;

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
        else if (selectedCard != card) // 選択中, かつ他のカードをクリック
        {
            try
            {
                //commonSymbol[card.CardNum].cardAction.Activate(OwnerType.PLAYER);
            }
            catch
            {
                //Debug.LogWarning(commonSymbol[card.CardNum].symbolSprite.name + " に効果が設定されていません。");
            }

            //Trash(selectedCard);
            //Trash(card);

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

        if (GUILayout.Button("再配置"))
        {
            //t.RearrangeHand();
        }
    }
}
#endif
