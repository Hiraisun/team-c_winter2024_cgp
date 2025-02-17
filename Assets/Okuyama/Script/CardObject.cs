using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    public int cardID{get; private set;} // カードID 0~12
    public List<int> symbolID = new List<int>(); // シンボルのリスト 0~12 size:4

    //カードの位置情報はコレで扱う
    //後々に演出などで情報が増える想定。
    public struct CardPos
    {
        public Vector3 center;
        public float angle;
    }

    private CardPos handPos; //手札として持つときの位置
    private CardPos hoverPos; //ホバー時の位置

    private enum CardState
    {
        Hand,
        Hover,
        selected,
    }
    private CardState state = CardState.Hand;

    public void Initialize(int cardID)
    {
        this.cardID = cardID;
        symbolID = CardManager.instance.GetSymbolList(cardID);

        //TODO:カードの見た目
    }

    void Update()
    {
        CardPos target;    //Lerp用
        switch (state)
        {
            case CardState.Hand:
                target = handPos;
                break;
            case CardState.Hover:
                target = hoverPos;
                break;
            case CardState.selected:
                target = handPos;
                break;
            default:
                target = handPos;
                break;
        }
        //Lerp
        transform.position = Vector3.Lerp(transform.position, target.center, 0.1f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, target.angle), 0.1f);
    }

    public void SetHandPos(CardPos pos)
    {
        handPos = pos;
    }
}
