using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAdjustSample : MonoBehaviour
{
    // 手札の位置調整用パラメータ
    [SerializeField] private Vector3 HandCenter = new Vector3(0, -16, 0); //中心
    [SerializeField] private float HandRadius = 14;  //円弧半径
    [SerializeField] private float HandAngle = 21;   //角度 真上を中心に左右に広がる

    [SerializeField] private GameObject CardPrefab;
    private Vector3 CardSize
    {
        get {
            try
            {
                // Assetから自動で取得
                // 良い感じに書き換えて
                return CardPrefab.GetComponent<SpriteRenderer>().bounds.size;
            }
            catch
            {
                return new Vector3(2.75f, 3.87f, 0);
            }
        }
    }

    public struct CardPos
    {
        public Vector3 center;
        public float angle;
    }


    // 手札枚数に応じた手札の位置を計算
    // TODO:手札枚数が少ないときの見栄えが悪い。要調整
    public List<CardPos> CalculateHandPos(int handNum)
    {
        List<CardPos> cardPosList = new List<CardPos>();

        // 例外処理
        if (handNum == 0) return cardPosList;
        if (handNum == 1)
        {
            CardPos cardPos = new CardPos();
            cardPos.center = HandCenter + new Vector3(0, HandRadius, 0);
            cardPos.angle = 0;
            cardPosList.Add(cardPos);
            return cardPosList;
        }

        //ほんへ
        for (int i = 0; i < handNum; i++)
        {
            CardPos cardPos = new CardPos();

            float angle = Mathf.Lerp(-HandAngle, HandAngle, (float)i / (handNum - 1));

            // 中心座標計算
            float CenterX = HandCenter.x + HandRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
            float CenterY = HandCenter.y + HandRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float CenterZ = i * -0.1f; // 右の手札ほど手前に (UIデザイン考慮して検討)

            cardPos.center = new Vector3(CenterX, CenterY, CenterZ);
            cardPos.angle = -angle;
            cardPosList.Add(cardPos);
        }
        return cardPosList;
    }



    void OnDrawGizmos()
    {
        // デモ用にカード配置を描画
        List<CardPos> cardPosList = CalculateHandPos(5);
        Gizmos.color = Color.red;
        foreach (var cardPos in cardPosList)
        {
            DrawCardGizmo(cardPos);
        }
    }

    // Gizmoで角度を含めて描画する
    private void DrawCardGizmo(CardPos cardPos)
    {
        Matrix4x4 original = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(
            cardPos.center,
            transform.rotation * Quaternion.Euler(0, 0, cardPos.angle), // オブジェクトの回転に追加
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, CardSize);
        Gizmos.matrix = original;
    }
}

