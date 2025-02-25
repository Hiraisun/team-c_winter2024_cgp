using UnityEngine;

[CreateAssetMenu(fileName = "SymbolData", menuName = "ScriptableObject/SymbolData")]
public class SymbolData : ScriptableObject
{
    [Header("シンボル名")]
    public string symbolName;

    [Header("シンボルに対応するアクションの説明")]
    public string description;

    [Header("シンボルに対応するアクション")]
    public ActionScriptableObject action;

    [Header("シンボルのスプライト")]
    public Sprite symbolSprite;
}
