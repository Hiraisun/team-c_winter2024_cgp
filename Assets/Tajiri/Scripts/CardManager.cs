using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR;
using Unity.VisualScripting;

public class CardManager : MonoBehaviour
{
    // 使用するすべてのカード
    public List<List<int>> Deck{ get; private set; }

    // 全カードのゲームオブジェクト配列
    private GameObject[] cardObjs;

    // 全Cardインスタンスの配列
    private Card[] cardCmps;

    [SerializeField]
    private PlayerResourceManager playerResourceManager;

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

    // ドローイベント
    private event Action OnCardDrawn;
    public void AddCardDrawnListener(Action listener) => OnCardDrawn += listener;
    // 手札選択イベント
    private event Action<Card> OnCardSelected;
    public void AddCardSelectedListener(Action<Card> listener) => OnCardSelected += listener;
    // 手札選択解除イベント
    private event Action OnCardDeselected;
    public void AddCardDeselectedListener(Action listener) => OnCardDeselected += listener;
    // 手札使用イベント
    private event Action OnCardUsed;
    public void AddCardUsedListener(Action listener) => OnCardUsed += listener;
    // 使用失敗イベント 
    private event Action OnCardUseFailed;
    public void AddCardUseFailedListener(Action listener) => OnCardUseFailed += listener;


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
    public bool DrawCard()
    {
        // 上限処理
        if (cardCmps.Count(c => c.IsCardInHand) >= MAX_HAND_CARDS)
        {
            Debug.LogWarning("DrawCard: 手札が最大枚数に達しています。");
            return false;
        }

        // Deckのカードを取得
        var inDeckCards = cardCmps.Where(c => c.State == Card.CardState.InDeck).ToList();

        if (inDeckCards.Count <= 0)
        {
            Debug.LogWarning("DrawCard: デッキにカードがありません。");
            return false;
        }

        // 正常処理
        Card newCard = inDeckCards[UnityEngine.Random.Range(0, inDeckCards.Count)];
        newCard.Draw();
        OnCardDrawn?.Invoke();
        RearrangeHand();
        return true;
    }

    /// <summary>
    /// カードを選択する ----------------------(2)
    /// </summary>
    private void SelectCard(Card card)
    {
        card.Select();
        selectedCard = card;
        OnCardSelected?.Invoke(card);
    }

    /// <summary>
    /// カードの選択を解除する-----------------(3)
    /// </summary>
    private void DeselectCard()
    {
        selectedCard.Deselect();
        OnCardDeselected?.Invoke();
        selectedCard = null;
    }

    /// <summary>
    /// 指定した2枚のカードを使用する -----------------(4)
    /// </summary>
    private void TryActivateCards(Card card1, Card card2)
    {
        // 重複するシンボルを取得
        var commonSymbolIDs = card1.SymbolsHashSet.Intersect(card2.SymbolsHashSet).ToList();

        // シンボルの数が1つの場合
        if (commonSymbolIDs.Count == 1)
        {
            // シンボルに対応する効果を発動
            CardEffectBase effect = AllSymbolData[commonSymbolIDs[0]].cardAction;

            if(playerResourceManager.ConsumeMana(effect.ManaCost))
            {
                // 正常処理
                effect.Activate(OwnerType.PLAYER);
                card1.Use();
                card2.Use();
                selectedCard = null;
                OnCardUsed?.Invoke();
                RearrangeHand();
            }
            else
            {
                Debug.Log("TryActivateCards: マナ不足");
                OnCardUseFailed?.Invoke();
            }

        }
        else
        {
            Debug.LogError("ActivateCards: 重複シンボルが1つじゃない。"+ commonSymbolIDs.ToString());
        }
    }

    /// <summary>
    /// 選択中のカードを捨てる-----------------(5)
    /// TODO:リソーステストのために暫定的にpublic
    /// </summary>
    public bool TrashSelectedCard()
    {
        if (selectedCard == null)
        {
            Debug.LogWarning("TrashSelectedCard: 選択中のカードがありません。");
            return false;
        }
        
        selectedCard.Trash();
        OnCardDeselected?.Invoke();
        selectedCard = null;
        RearrangeHand();
        return true;
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
            TryActivateCards(selectedCard, card);
        }
        else // 同じカードをクリック
        {
            DeselectCard();
        }
    }

#if UNITY_EDITOR
    // 各種パラメータ設定用Gizmo
    void OnDrawGizmosSelected()
    {
        // 手札位置
        Vector3 cardSize = cardPrefab.GetComponent<RectTransform>().sizeDelta;
        var (positions, rotations) = CardAlgorithms.CalculateHandPos(5, handPosConfig);
        for (int i = 0; i < positions.Count; i++)
        {
            DrawCardGizmo(positions[i], rotations[i].eulerAngles.z, cardSize);
        }

        // ドロー位置
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(drawPos, cardSize);

        // 使用位置
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(usedPos, 0.5f);

        // トラッシュ位置
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(trashPos, 0.5f);
    }
    // 角度付きのWireCubeを描画
    private void DrawCardGizmo(Vector3 center, float angle, Vector3 size)
    {
        Matrix4x4 original = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(
            center,
            transform.rotation * Quaternion.Euler(0, 0, angle), // オブジェクトの回転に追加
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = original;
    }
#endif


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

        if (GUILayout.Button("選択中のカードを捨てる"))
        {
            t.TrashSelectedCard();
        }
    }
}
#endif
