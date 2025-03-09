using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// カードのシンボルの挙動を制御する
/// シンボルはGameObejctにアタッチされる
/// </summary>
public class CardSymbol : MonoBehaviour
{
    // マウスのホバー（入）イベント
    private event Action<CardSymbol> OnSymbolMouseEnter;
    public void AddSymbolMouseEnterListener(Action<CardSymbol> listener)
        => OnSymbolMouseEnter += listener;

    // マウスのホバー（出）イベント
    private event Action<CardSymbol> OnSymbolMouseExit;
    public void AddSymbolMouseExitListener(Action<CardSymbol> listener)
        => OnSymbolMouseExit += listener;

    // シンボル名を表示するTMP
    private TextMeshPro symbolNameText
        => this.GetComponentInChildren<TextMeshPro>();

    private string symbolName;          // シンボル名

    private string symbolDescription;   // シンボルの説明

    public string SymbolDescription { get => symbolDescription; }

    private event Action<Card> OnSymbolClicked;
    public void AddCardClickedListener(Action<Card> listener) => OnSymbolClicked += listener;

    /// <summary>
    /// シンボルのデータを取得
    /// </summary>
    /// <param name="cardNum"></param>
    /// <param name="cardManager"></param>
    public void Initialize(int symbolIndex, CardManager cardManager)
    {
        this.symbolName = cardManager.AllSymbolData[symbolIndex].symbolName;
        this.symbolDescription = cardManager.AllSymbolData[symbolIndex].description;

        // TMPにシンボル名を設定
        this.symbolNameText.text = symbolName;
    }

    // マウスホバーの処理
    private void OnMouseEnter()
    {
        OnSymbolMouseEnter?.Invoke(this);
        //transform.DOScale(Vector2.one * 1.1f, 0.2f);
    }

    private void OnMouseExit()
    {
        OnSymbolMouseExit?.Invoke(this);
        //transform.DOScale(Vector2.one, 0.2f);
    }
}
