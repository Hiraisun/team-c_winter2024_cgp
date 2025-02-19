using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "SymbolData", menuName = "SymbolData", order = 51)]
public class SymbolData : ScriptableObject
{
    public string symbolName;
    public Sprite symbolSprite;
}
