using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Vector3 HandCenter;
    [SerializeField] private float HandRadius;
    [SerializeField] private float HandAngle;


    [SerializeField] private Vector3 CardSize; 

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public struct CardPos
    {
        public Vector3 CenterPos;
        public float Angle;
    }
    // 手札枚数に応じた手札の位置を計算
    List<CardPos> CalculateHandPos(int handNum)
    {
        List<CardPos> cardPosList = new List<CardPos>();
        for (int i = 0; i < handNum; i++)
        {
            CardPos cardPos = new CardPos();

            // 各手札の角度計算
            float angle = Mathf.Lerp(-HandAngle, HandAngle, (float)i / (handNum - 1));
            
            // 中心座標計算
            float CenterX = HandCenter.x + HandRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
            float CenterY = HandCenter.y + HandRadius * Mathf.Cos(angle * Mathf.Deg2Rad);

            cardPos.CenterPos = new Vector3(CenterX, CenterY, 0);
            cardPos.Angle = angle;
            cardPosList.Add(cardPos);
        }
        return cardPosList;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // デモ用にカード配置を描画
        List<CardPos> cardPosList = CalculateHandPos(5);
        foreach (var cardPos in cardPosList)
        {
            Matrix4x4 original = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(
                cardPos.CenterPos,
                transform.rotation * Quaternion.Euler(0, 0, -cardPos.Angle), // オブジェクトの回転に追加
                Vector3.one
            );
            Gizmos.DrawWireCube(Vector3.zero, CardSize);
            Gizmos.matrix = original;
        }
    }
}
