using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardState
{
    Init,       // �������,���g�p
    Hand,       // ��D(��I��)
    Selected,   // �I��
    Used        // �g�p�ς�, ���o���I������j��
}


public class Card : MonoBehaviour
{
    // SerializeField-----------------------------------------

    [SerializeField, Header("�V���{����\������SpriteRenderer")]
    private SpriteRenderer[] sr;

    [SerializeField]
    private CardManager cm;

    // �v���p�e�B----------------------------------------------

    public int cardNum{ get; private set; } // �J�[�hID

    public List<int> symbolIndices{ get; private set; } // �V���{��ID���X�g
    public List<SymbolData> symbolsData{ get; private set; } // �V���{���f�[�^���X�g
    
    public CardState cardState{ get; private set; } // �J�[�h�̏��

    /// <summary>
    /// ������
    /// �C���X�^���X�����ɕK���Ă� �����������͂����Ŋ���
    /// </summary>
    public void Initialize(int cardNum, CardManager cardManager)
    {
        cm = cardManager;
        cardState = CardState.Hand;

        // �J�[�hID���w��
        this.cardNum = cardNum;

        // �V���{�����擾
        symbolIndices = cm.GetSymbols(cardNum);
        symbolsData = new();
        foreach (int index in symbolIndices)
        {
            symbolsData.Add(cm.GetSymbolData(index));
        }

        InitializeSymbolImage(); // �V���{���摜������
        InitializeUnitName(); // ���j�b�g��������
    }

    // �r�W���A�������n�͕�class�ɂ��Ă����������B�P��ӔC�̌���!!
    // Symbol��Sprite��Image�ɓK�p
    private void InitializeSymbolImage()
    {
        for (int i = 0; i < symbolsData.Count; i++)
        {
            sr[i].sprite = symbolsData[i].symbolSprite;
        }
    }
    // �V���{���̉E�Ƀ��j�b�g����\������??
    private void InitializeUnitName()
    {
        // TODO:������
    }



    // ���̃I�u�W�F�N�g���N���b�N���ꂽ�Ƃ�
    private void OnMouseDown()
    {
        // CM�ɒʒm
        cm.NotifyCardClicked(this);

        // State����Ȃǂ�Card���ƒf�ł��Ȃ��ACM�ɔC����
        // �s�k���o���ɑI���ł��Ă��܂����̃o�O���l��
    }

    /// <summary>
    /// ��D���ł̈ʒu����
    /// </summary>
    public void MoveToHandPos(Vector3 position)
    {
        transform.position = position;

        // TODO:Lerp�Ƃ�
    }


    // State����----------------------------------------------
    // ���o�ȂǂŔώG�ɂȂ�悤�Ȃ�AState�p�^�[���ɏ���������Ƃ�������
    // �Q�l�Fhttps://qiita.com/AsahinaKei/items/ce8e5d7bc375af23c719

    /// <summary>
    /// �h���[���Ȃ� ��D�ɂ���
    /// </summary>
    public void ToHand()
    {
        cardState = CardState.Hand;
        
        // TODO:��D�ʒu�Ɏ����Ă��鏈���Ƃ�
    }

    /// <summary>
    /// �J�[�h��I����ԂɈڍs
    /// </summary>
    public void Select(){
        cardState = CardState.Selected;

        // TODO:�J�[�h�n�C���C�g����
        // TODO:���炷
        // TODO:��D�̑Ή��V���{�������点��
    }

    /// <summary>
    /// �I����Ԃ��L�����Z��
    /// </summary>
    public void Deselect(){
        cardState = CardState.Hand;

        // TODO:�F�X�ȉ��o����������
    }

    /// <summary>
    /// �J�[�h���g�p�ς݂ɂ���
    /// </summary>
    public void Use(){
        cardState = CardState.Used;
        
        // TODO:�R���Đo�ɂȂ��ď�����Ƃ�?

        Destroy(gameObject);
    }

}




