
using System;
using UnityEngine;
using TMPro;

/// <summary>
/// カードのシンボルの挙動を扱う
/// </summary>
public class CardSymbol : MonoBehaviour
{
    private event Action<CardSymbol> OnSymbolMouseEnter;
    public void AddSymbolMouseEnterListener(Action<CardSymbol> listener) => OnSymbolMouseEnter += listener;

    private event Action<CardSymbol> OnSymbolMouseExit;
    public void AddSymbolMouseExitListener(Action<CardSymbol> listener) => OnSymbolMouseExit += listener;

    [SerializeField]
    private TextMeshPro symbolNameText;

    private string symbolName;

    public void Initialize(int symbolIndex)
    {
        
    }

    // マウスホバー時にイベント発行
    private void OnMouseEnter()
    {
        OnSymbolMouseEnter?.Invoke(this);
    }

    private void OnMouseExit()
    {
        OnSymbolMouseExit?.Invoke(this);
    }
}
