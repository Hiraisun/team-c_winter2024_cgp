using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// シンボルのUI関連
/// </summary>
public class UISymbol : UIBase
{
    // マウスのホバー（入）イベント
    private event Action<string> OnSymbolMouseEnter;
    public void AddSymbolMouseEnterListener(Action<string> listener)
        => OnSymbolMouseEnter += listener;

    // マウスのホバー（出）イベント
    private event Action OnSymbolMouseExit;
    public void AddSymbolMouseExitListener(Action listener)
        => OnSymbolMouseExit += listener;

    // シンボル名を表示するTMP
    private TextMeshPro symbolNameText;

    private string symbolName;              // シンボル名

    private string symbolDescription;       // シンボルの説明

    /// <summary>
    /// シンボルのデータを取得
    /// </summary>
    public void Initialize(int symbolIndex, CardManager cardManager)
    {
        this.symbolName = cardManager.AllSymbolData[symbolIndex].symbolName;
        this.symbolDescription = cardManager.AllSymbolData[symbolIndex].description;

        // TMPにシンボル名を設定
        this.symbolNameText = this.GetComponentInChildren<TextMeshPro>();
        this.symbolNameText.text = symbolName;
    }

    protected override void OnCursorEnter()
    {
        OnSymbolMouseEnter?.Invoke(symbolDescription);
    }

    protected override void OnCursorExit()
    {
        OnSymbolMouseExit?.Invoke();
    }
}
