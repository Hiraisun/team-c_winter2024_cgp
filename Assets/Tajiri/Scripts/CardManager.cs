using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR;

public class CardManager : MonoBehaviour
{
    // 使用するすべてのカード
    public List<List<int>> Deck{ get; private set; }

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

    [SerializeField, Header("手札位置の調整パラメータ")]
    private CardAlgorithms.HandPosConfig handPosConfig = new()
    {
        handPos = new Vector3(0, 0, 0),
        handRadius = 5f,
        handMaxAngle = 45f,
        gapSizeMagnification = 3f
    };

    [Space(10)]
    [SerializeField, Header("ドロー時出現位置")]
    private Vector3 drawPos = new(-10, 0, 0);
    public Vector3 DrawPos { get => drawPos; }
    [SerializeField, Header("使用時移動先")]
    private Vector3 usedPos = new(10, 0, 0);
    public Vector3 UsedPos { get => usedPos; }
    [SerializeField, Header("トラッシュ時移動先")]
    private Vector3 trashPos = new(0, 10, 0);
    public Vector3 TrashPos { get => trashPos; }

    // ScriptableObjectからロードしたシンボルデータ
    private SymbolData[] allSymbolData;
    public SymbolData[] AllSymbolData{ get => allSymbolData; }

    private int TOTAL_CARD_AND_SYMBOL => SYMBOL_PER_CARD * SYMBOL_PER_CARD - SYMBOL_PER_CARD + 1; 
    // カードの枚数とシンボルの数は同じ

    private int[] HandNums => cardCmps.Select(card => card.CardNum).ToArray();


    // 手札選択イベント
    private event Action<Card> OnCardSelected;
    public void AddCardSelectedListener(Action<Card> listener) => OnCardSelected += listener;
    // 手札選択解除イベント
    private event Action<Card> OnCardDeselected;
    public void AddCardDeselectedListener(Action<Card> listener) => OnCardDeselected += listener;
    // 手札使用イベント
    private event Action OnCardUsed;
    public void AddCardUsedListener(Action listener) => OnCardUsed += listener;


    // 選択中のカード
    private Card selectedCard = null;




    // 初期化処理
    private void Awake()
    {
        InitializeArrays();
        Deck = CardAlgorithms.GenerateDobbleCardsList(SYMBOL_PER_CARD);
        GenerateCardsObj();
    }

    // メモリ確保
    private void InitializeArrays()
    {
        allSymbolData = new SymbolData[TOTAL_CARD_AND_SYMBOL];
        cardObjs = new GameObject[TOTAL_CARD_AND_SYMBOL];
        cardCmps = new Card[TOTAL_CARD_AND_SYMBOL];

        allSymbolData = Resources.LoadAll<SymbolData>("Symbols");
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
            cardCmps[i].Initialize(i, this);       // 初期化
            cardCmps[i].AddCardClickedListener(HandleCardClicked); // クリックイベントの登録
        }
    }

    /// <summary>
    /// カードを1枚引く   ---------------------(1)
    /// </summary>
    public void DrawCard()
    {
        // 上限処理
        if (cardCmps.Count(c => c.IsCardInHand) >= MAX_HAND_CARDS)
        {
            Debug.LogWarning("手札が最大枚数に達しています。");
            return;
        }

        // Deckのカードを取得
        var inDeckCards = cardCmps.Where(c => c.State == Card.CardState.InDeck).ToList();

        if (inDeckCards.Count > 0)
        {
            Card newCard = inDeckCards[UnityEngine.Random.Range(0, inDeckCards.Count)];
            newCard.Draw();
            RearrangeHand();
        }
        else
        {
            Debug.LogWarning("デッキにカードがありません。");
        }
    }

    /// <summary>
    /// カードを選択する ----------------------(2)
    /// </summary>
    public void SelectCard(Card card)
    {
        card.Select();
        selectedCard = card;
        OnCardSelected?.Invoke(card);
    }

    /// <summary>
    /// カードの選択を解除する-----------------(3)
    /// </summary>
    public void DeselectCard(Card card)
    {
        if(card != selectedCard)
        {
            Debug.LogWarning("選択中のカードではありません。");
            return;
        }
        card.Deselect();
        selectedCard = null;
        OnCardDeselected?.Invoke(card);
    }

    /// <summary>
    /// 指定した2枚のカードを使用する -----------------(4)
    /// </summary>
    public void ActivateCards(Card card1, Card card2)
    {
        // 重複するシンボルを取得
        var commonSymbolIDs = card1.SymbolsHashSet.Intersect(card2.SymbolsHashSet).ToList();

        // シンボルの数が1つの場合
        if (commonSymbolIDs.Count == 1)
        {
            // シンボルに対応する効果を発動
            try{
                AllSymbolData[commonSymbolIDs[0]].cardAction.Activate(OwnerType.PLAYER);
            }
            catch (NullReferenceException)
            {
                Debug.LogWarning("効果未設定 : " + AllSymbolData[commonSymbolIDs[0]].description);
            }
            card1.Use();
            card2.Use();
            selectedCard = null;
            OnCardUsed?.Invoke();
            RearrangeHand();
        }
        else
        {
            Debug.LogError("重複シンボルが1つじゃない。"+ commonSymbolIDs.ToString());
        }
    }


    /// <summary>
    /// 手札のカードを再配置する
    /// </summary>
    private void RearrangeHand()
    {
        var cardsInHand = cardCmps.Where(c => c.IsCardInHand).ToList();
        var (positions, rotations) = CardAlgorithms.CalculateHandPos(cardsInHand.Count, handPosConfig);

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].SetHandPosition(positions[i], rotations[i]);
        }
    }

    /// <summary>
    /// カードクリックイベントのハンドラメソッド。
    /// 
    /// !!! イベントハンドリング (クリック時の処理の振り分け) のみを行う !!!
    /// 気を抜くとここが肥大化する。分散しよう。
    /// 
    /// ((そもそもイベントである必要がない説がある。))
    /// </summary>
    private void HandleCardClicked(Card card)
    {
        if (selectedCard == null) // 何も選択中じゃない
        {
            SelectCard(card);
        }
        else if (selectedCard != card) // 選択中, かつ他のカードをクリック
        {
            ActivateCards(selectedCard, card);
        }
        else // 同じカードをクリック
        {
            DeselectCard(card);
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
            t.DrawCard();
        }

        if (GUILayout.Button("再配置"))
        {
            //t.RearrangeHand();
        }
    }
}
#endif
