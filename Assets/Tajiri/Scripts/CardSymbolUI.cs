using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シンボルのUIの挙動を扱う
/// </summary>
public class CardSymbolUI : MonoBehaviour
{
    [SerializeField, Header("シンボル説明ポップアップウィンドウ")]
    private GameObject descriptionWindow;

    public void Initialize(CardManager cardManager)
    {
        
    }
}
