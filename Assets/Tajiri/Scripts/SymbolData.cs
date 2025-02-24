using UnityEngine;

[CreateAssetMenu(fileName = "SymbolData", menuName = "ScriptableObject/SymbolData")]
public class SymbolData : ScriptableObject
{
    [Header("�V���{����")]
    public string symbolName;

    [Header("�V���{���ɑΉ�����A�N�V�����̐���")]
    public string description;

    [Header("�V���{���ɑΉ�����A�N�V����")]
    public ActionScriptableObject action;

    [Header("�V���{���̃X�v���C�g")]
    public Sprite symbolSprite;

    public bool isHighlighted = false;
}
