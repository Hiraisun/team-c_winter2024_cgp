using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class Card : MonoBehaviour
{
    [Header("�J�[�h�ԍ�")]
    public int cardNum;
    // CardManager��Symbol������
    [HideInInspector]
    public List<SymbolData> symbols;

    [SerializeField, Header("�V���{����\������SpriteRenderer")]
    private SpriteRenderer[] sr;

    private CardManager cm;

    public List<int> symbolIndices = new();

    [HideInInspector]
    public bool isSelected = false;

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
            sr[i].sprite = symbols[i].symbolSprite;
        }
    }

    // ���̃I�u�W�F�N�g���N���b�N���ꂽ�Ƃ�
    private void OnMouseDown()
    {
        if (isSelected) return;

        isSelected = true;

        cm.selectedCards.Add(this.GetComponent<Card>());
        cm.UseCard();
    }
}




