using UnityEngine;
using System;
using DG.Tweening;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;


/// <summary>
/// カードオブジェクトの挙動を扱う
/// </summary>
public class Card : MonoBehaviour
{
    // ID
    public int CardNum { get; private set; }

    // カードの効果説明
    public string EffectDiscription { get; private set; }

    public enum CardState{
        InDeck,
        Hand_Unselected,
        Hand_Selected,
        Used,
        Trashed
    }
    private CardState state; // カードの状態
    public CardState State { get => state; }

    // 手札にあるかどうか ( Hand_Unselected または Hand_Selected かどうか)
    public bool IsCardInHand { get => state == CardState.Hand_Unselected || state == CardState.Hand_Selected; }

    [SerializeField, Header("シンボルを表示するSpriteRenderer")]
    private SpriteRenderer[] sr;

    // カードがクリックされたときのイベント
    private event Action<Card> OnCardClicked;
    public void AddCardClickedListener(Action<Card> listener) => OnCardClicked += listener;

    // カードマネージャーの参照
    private CardManager cardManager;

    // シンボルリスト
    private List<int> symbols;
    // シンボルのHashSet
    public HashSet<int> SymbolsHashSet { get; private set; }

    // 手札として持つ際の位置情報キャッシュ
    private Vector3 handPosition;
    private Quaternion handRotation;
    // TODO:選択状態での位置情報 (暫定的に少し上に)
    private Vector3 selectedPosition { get => handPosition + Vector3.up*1f; }
    private Quaternion selectedRotation { get => handRotation; }

    /// <summary>
    /// 初期化処理を一元化したメソッド
    /// </summary>
    public void Initialize(int cardNum, CardManager cardManager)
    {
        this.cardManager = cardManager;
        this.CardNum = cardNum;

        state = CardState.InDeck; //初期状態
        transform.position = cardManager.DrawPos;

        // カードデータ初期化
        this.symbols = cardManager.Deck[CardNum];
        this.SymbolsHashSet = new HashSet<int>(symbols);

        // イベントハンドラの登録
        cardManager.AddCardSelectedListener(HandleCardSelected);
        cardManager.AddCardDeselectedListener(HandleCardDeselected);
        cardManager.AddCardUsedListener(HandleCardUsed);

        // シンボルスプライトの初期化
        for (int i = 0; i < sr.Length; i++)
        {
            int symbolIndex = symbols[i];
            sr[i].sprite = cardManager.AllSymbolData[symbolIndex].symbolSprite;
        }

        // TODO: 効果説明の初期化
    }

    /// <summary>
    /// 手札座標の再セット
    /// </summary>
    public void SetHandPosition(Vector3 position, Quaternion rotation)
    {
        handPosition = position;
        handRotation = rotation;

        // 手札状態なら移動
        if (state == CardState.Hand_Unselected) {
            MoveTo(handPosition, handRotation);
        }
        else if (state == CardState.Hand_Selected) {
            MoveTo(selectedPosition, selectedRotation);
        }
    }

    /// <summary>
    /// カードを手札に加える  -------------(1)
    /// </summary>
    public void Draw()
    {
        if(state!=CardState.InDeck){
            Debug.LogError("カードがデッキにない");
            return;
        }

        state = CardState.Hand_Unselected;
    }

    /// <summary>
    /// 手札のカードを選択する -------------(2)
    /// </summary>
    public void Select()
    {
        if(state!=CardState.Hand_Unselected){
            Debug.LogError("カードが手札にない");
            return;
        }

        state = CardState.Hand_Selected;

        // 選択時演出
        MoveTo(selectedPosition, selectedRotation);
        //TODO:枠線
    }

    /// <summary>
    /// 手札のカードを選択解除する-------------(3)
    /// </summary>
    public void Deselect()
    {
        if(state!=CardState.Hand_Selected){
            Debug.LogError("カードが選択されていない");
            return;
        }

        state = CardState.Hand_Unselected;

        // 選択解除演出
        MoveTo(handPosition, handRotation);
    }

    /// <summary>
    /// カードを使用する -------------(4)
    /// </summary>
    public async void Use()
    {
        if(state!= CardState.Hand_Selected  && state!= CardState.Hand_Unselected){
            Debug.LogError("カードが手札にない状態で使用された");
            return;
        }

        state = CardState.Used;
        
        // 使用時演出待機
        await UseVisualEffect();

        state = CardState.InDeck;
        transform.position = cardManager.DrawPos;
    }
    // カード使用時演出
    private async UniTask UseVisualEffect()
    {
        // TODO:暫定的にぐるぐる回してます
        transform.DOKill();
        Sequence sequence = DOTween.Sequence()
            .Append(transform.DOMove(cardManager.UsedPos, 1f))
            .Join(transform.DORotate(new Vector3(0, 0, 1080), 2f, RotateMode.FastBeyond360).SetEase(Ease.InQuad))
            .Join(transform.DOScale(Vector3.zero, 2f).SetEase(Ease.InQuad));
        await sequence.Play().ToUniTask();

        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// カードを捨てる -------------(5)
    /// </summary>
    public async void Trash()
    {
        if(state!= CardState.Hand_Selected  && state!= CardState.Hand_Unselected){
            Debug.LogError("カードが手札にない状態で捨てられた");
            return;
        }

        state = CardState.Trashed;
        await transform.DOMove(cardManager.TrashPos, 1f);

        state = CardState.InDeck;
        transform.position = cardManager.DrawPos;

    }



    /// <summary>
    /// カード選択のイベントハンドラ 何らかのカードが選択されたとき
    /// </summary>
    public void HandleCardSelected(Card selectedCard)
    {
        if(selectedCard != this)// 他のカードが選択された
        {
            // シンボルハイライト演出
            StartSymbolHighlight(selectedCard);
        }
    }
    /// <summary>
    /// カード選択解除のイベントハンドラ
    /// </summary>
    public void HandleCardDeselected()
    {
        // シンボルハイライト演出終了
        FinishSymbolHighlight();
    }
    /// <summary>
    /// カード使用のイベントハンドラ
    /// </summary>
    public void HandleCardUsed()
    {
        // シンボルハイライト演出終了
        FinishSymbolHighlight();
    }
    
    /// <summary>
    /// シンボルハイライト演出を開始
    /// </summary>
    private void StartSymbolHighlight(Card selectedCard)
    {
        // 一致判定
        int highlightSymbol = selectedCard.SymbolsHashSet.Intersect(SymbolsHashSet).FirstOrDefault();
        int highlightSymbolIndex = symbols.IndexOf(highlightSymbol);

        if (highlightSymbolIndex == -1) return;
        sr[highlightSymbolIndex].DOKill();
        sr[highlightSymbolIndex].DOColor(Color.green, 0.5f);
    }
    /// <summary>
    /// シンボルハイライト演出を終了
    /// </summary>
    private void FinishSymbolHighlight()
    {
        foreach(SpriteRenderer _sr in sr)
        {
            _sr.DOKill();
            _sr.DOColor(Color.white, 0.5f);
        }
    }

    /// <summary>
    /// カードを指定した座標/回転角に移動する演出
    /// </summary>
    private void MoveTo(Vector3 position, Quaternion rotation, float duration = 0.3f)
    {
        DOTween.Kill(transform);
        transform.DOMove(position, duration);
        transform.DORotateQuaternion(rotation, duration);
    }

    //カプセル化する意味がなくなる...
    //public void SetCardNum(int value) => CardNum = value;
    //public void SetCardInHand(bool value) => IsCardInHand = value;
    //public void SetCardDescription(string value) => EffectDiscription = value;

    // このオブジェクトがクリックされたとき
    private void OnMouseDown()
    {
        if (IsCardInHand) OnCardClicked?.Invoke(this);
    }

    // マウスホバー時の拡大縮小
    private void OnMouseEnter()
    {
        if (IsCardInHand) transform.DOScale(Vector2.one * 1.1f, 0.2f);
    }
    private void OnMouseExit()
    {
        if (IsCardInHand) transform.DOScale(Vector2.one, 0.2f);
    }
}




