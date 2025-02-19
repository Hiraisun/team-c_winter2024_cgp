using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Card : MonoBehaviour
{
    // CardManager��Symbol������
    [HideInInspector]
    public List<SymbolData> symbols = new List<SymbolData>();

    [SerializeField, Header("�V���{����\������Image")]
    private Image[] symbolImages;

    private CardManager cm;

    private void OnEnable()
    {
        // CardManager���擾
        cm = GameObject.FindAnyObjectByType<CardManager>();
    }

    // Symbol��Sprite��Image�ɓK�p
    public void ApplySymbols()
    {
        for (int i = 0; i < symbols.Count; i++)
        {
            symbolImages[i].sprite = symbols[i].symbolSprite;
        }
    }

    // ���̃I�u�W�F�N�g���N���b�N���ꂽ�Ƃ�
    private void OnMouseDown()
    {
        cm.selectedCardCount++;
    }
}




