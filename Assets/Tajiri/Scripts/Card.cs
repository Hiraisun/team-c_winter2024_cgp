using UnityEngine;
using System;
using DG.Tweening;
using System.Linq;
using System.Collections.Generic;


/// <summary>
/// カードオブジェクトの挙動を扱う
/// </summary>
public class Card : MonoBehaviour
{
    // ID
    public int CardNum { get; private set; }

    // カードの効果説明
    public string EffectDiscription { get; private set; }

    // 手札にあるかどうか
    private bool isCardInHand;
    public bool IsCardInHand { get; private set; }

    [SerializeField, Header("シンボルを表示するSpriteRenderer")]
    private SpriteRenderer[] sr;

    // カードがクリックされたときに発火
    private event Action<Card> OnCardClicked;
    // イベントリスナーを追加
    public void AddCardClickedListener(Action<Card> listener) => OnCardClicked += listener;


    /// <summary>
    /// 初期化処理を一元化したメソッド
    /// </summary>
    public void Initialize(int cardNum, List<List<int>> deck, SymbolData[] allSymbols)
    {
        CardNum = cardNum;
        IsCardInHand = false; // 初期状態は手札外

        // シンボルスプライトの初期化
        for (int i = 0; i < sr.Length; i++)
        {
            int symbolIndex = deck[CardNum][i];
            sr[i].sprite = allSymbols[symbolIndex].symbolSprite;
        }

        // TODO: 効果説明の初期化
    }

    /// <summary>
    /// カードを手札に加える
    /// それに伴った演出も行う
    /// </summary>
    public void ToHand()
    {
        IsCardInHand = true;
    }



    public void SetHighlightedSymbol(List<List<int>> deck, List<int> matchingSymbols)
    {
        int highlightedSymbol = deck[CardNum].Intersect(matchingSymbols).DefaultIfEmpty(-1).First();
        int highlightedSymbolIndex = deck[CardNum].IndexOf(highlightedSymbol);

        if (highlightedSymbolIndex == -1) return;
        sr[highlightedSymbolIndex].DOKill();
        sr[highlightedSymbolIndex].DOColor(Color.green, 1f);
    }

    public void ResetSymbolsColor()
    {
        foreach(SpriteRenderer _sr in sr)
        {
            _sr.DOKill();
            _sr.DOColor(Color.white, 1f);
        }
    }

    /// <summary>
    /// カードを指定した座標/回転角に1秒かけて移動
    /// </summary>
    public void MoveTo(Vector3 position, Quaternion rotation)
    {
        this.transform.DOMove(position, 1f);
        this.transform.DORotateQuaternion(rotation, 1f);
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
        this.transform.DOScale(Vector2.one * 1.1f, 0.2f);
    }
    private void OnMouseExit()
    {
        this.transform.DOScale(Vector2.one, 0.2f);
    }
}




