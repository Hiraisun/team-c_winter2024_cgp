using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// シンボルのUI関連
/// </summary>
public class UISymbol : UIBase
{
    // シンボルマネージャー
    private UISymbolManager uISymbolManager;

    // シンボル名を表示するTMP
    private TextMeshPro symbolNameText;

    private string symbolName;              // シンボル名

    private string symbolDescription;       // シンボルの説明

    /// <summary>
    /// シンボルのデータを取得
    /// </summary>
    public void Initialize(int symbolIndex, CardManager cardManager, UISymbolManager uISymbolManager)
    {
        // UISymbolManagerを取得
        this.uISymbolManager = uISymbolManager;

        // シンボルの名称、説明文を取得
        this.symbolName = cardManager.AllSymbolData[symbolIndex].symbolName;
        this.symbolDescription = cardManager.AllSymbolData[symbolIndex].description;

        // TMPにシンボル名を設定
        this.symbolNameText = this.GetComponentInChildren<TextMeshPro>();
        this.symbolNameText.text = symbolName;
    }

    /// <summary>
    /// 説明用ウィンドウ表示開始
    /// </summary>
    protected override void OnCursorEnter()
    {
        uISymbolManager.OnCursorEnter(symbolDescription);
    }

    /// <summary>
    /// 説明用ウィンドウ表示終了
    /// </summary>
    protected override void OnCursorExit()
    {
        uISymbolManager.OnCursorExit();
    }
}
