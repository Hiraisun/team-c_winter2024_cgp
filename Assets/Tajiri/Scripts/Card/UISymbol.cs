using System;
using UnityEngine;
using TMPro;

/// <summary>
/// カードのシンボルの挙動を制御する
/// シンボルのオブジェクトにはBoxCollider2Dが必要
/// </summary>
public class UISymbol : MonoBehaviour
{
    // マウスのホバー（入）イベント
    private event Action<UISymbol> OnSymbolMouseEnter;
    public void AddSymbolMouseEnterListener(Action<UISymbol> listener)
        => OnSymbolMouseEnter += listener;

    // マウスのホバー（出）イベント
    private event Action<UISymbol> OnSymbolMouseExit;
    public void AddSymbolMouseExitListener(Action<UISymbol> listener)
        => OnSymbolMouseExit += listener;

    // シンボル名を表示するTMP
    private TextMeshPro symbolNameText
        => this.GetComponentInChildren<TextMeshPro>();

    private string symbolName;              // シンボル名
    public string SymbolName { get => symbolName; }

    private string symbolDescription;       // シンボルの説明
    public string SymbolDescription { get => symbolDescription; }

    private BoxCollider2D symbolCollider;   // シンボルの当たり判定

    private bool isInsidePrev = false;

    /// <summary>
    /// シンボルのデータを取得
    /// </summary>
    public void Initialize(int symbolIndex, CardManager cardManager)
    {
        this.symbolName = cardManager.AllSymbolData[symbolIndex].symbolName;
        this.symbolDescription = cardManager.AllSymbolData[symbolIndex].description;

        // TMPにシンボル名を設定
        this.symbolNameText.text = symbolName;

        // コライダーコンポーネントを取得
        try
        {
            symbolCollider = this.gameObject.GetComponent<BoxCollider2D>();
        }
        catch
        {
            Debug.LogWarning("UISymbol : " + this.gameObject.name + "のBoxCollider2Dを取得できません。");
        }
    }

    private void Update()
    {
        CursorDetector();
    }

    private void CursorDetector()
    {
        Bounds symbolBounds = symbolCollider.bounds;

        Vector3 mousePos = UICursor.Instance.CursorPos;

        bool isInsideCrr = symbolBounds.Contains(mousePos);

        if (isInsideCrr && !isInsidePrev)
        {
            OnSymbolMouseEnter?.Invoke(this);
        }
        
        if (!isInsideCrr && isInsidePrev)
        {
            OnSymbolMouseExit?.Invoke(this);
        }

        isInsidePrev = isInsideCrr;
    }
}
