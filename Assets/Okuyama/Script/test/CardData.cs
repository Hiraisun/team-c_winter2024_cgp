using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ステージに関する、スクリプトに影響を与える情報を扱う
/// </summary>
[CreateAssetMenu(fileName = "CardData", menuName = "ScriptableObjects/CardData")]
public class CardData : ScriptableObject
{

    public IActivatable activatable;
    public string cardName;
}
