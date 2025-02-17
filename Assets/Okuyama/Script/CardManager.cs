using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // 手札の位置調整用パラメータ
    [SerializeField] private Vector3 HandCenter;
    [SerializeField] private float HandRadius;
    [SerializeField] private float HandAngle;


    [SerializeField] private Vector3 CardSize;

    private List<int>[] dobbleCardArr;

    void Awake()
    {
        InitializeDobbleCardArr(4);
    }

    void Update()
    {

    }

    public void Draw()
    {
        Debug.Log("Draw");
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
            float CenterZ = i * -0.1f; // 右の手札ほど手前に (UIデザイン考慮して検討)

            cardPos.CenterPos = new Vector3(CenterX, CenterY, CenterZ);
            cardPos.Angle = angle;
            cardPosList.Add(cardPos);
        }
        return cardPosList;
    }

    // DpbbleCardList (カード番号とマークの対応表)の初期化
    private void InitializeDobbleCardArr(int numOfSymbols)
    {
        int n_1 = numOfSymbols - 1;
        List<List<int>> cards = new List<List<int>>();
        List<int> card;

        //最初の一枚
        card = new List<int>();
        for (int i = 0; i <= n_1; i++)
        {
            card.Add(i);
        }
        cards.Add(card);

        //次のn枚
        for (int j = 0; j < n_1; j++)
        {
            card = new List<int>();
            card.Add(0);
            for (int k = 0; k < n_1; k++)
            {
                card.Add(n_1 + n_1 * j + k + 1);
            }
            cards.Add(card);
        }

        // 残りの n² 枚
        for (int i = 1; i <= n_1; i++)
        {
            for (int j = 1; j <= n_1; j++)
            {
                card = new List<int>();
                card.Add(i);
                for (int k = 1; k <= n_1; k++)
                {
                    card.Add(n_1 + 1 + n_1 * (k - 1) + ((i - 1) * (k - 1) + j - 1) % n_1);
                }
                cards.Add(card);
            }
        }

        dobbleCardArr = new List<int>[cards.Count];
        for (int i = 0; i < cards.Count; i++)
        {
            dobbleCardArr[i] = cards[i];   //配列に写す
        }

        return;
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
