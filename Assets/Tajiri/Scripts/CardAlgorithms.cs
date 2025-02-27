using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardAlgorithms
{

    /// <summary>
    /// Dobbleカードリストを生成する
    /// staticなのでコンパイルで最適化される。たぶん。
    /// </summary>
    public static List<List<int>> GenerateDobbleCardsList(int symbolOnCard)
    {
        int n = symbolOnCard - 1;

        List<List<int>> resultDeck = new();

        // 1枚目のカード (0, 1, 2, ..., n)
        List<int> firstCard = new();
        for (int i = 0; i <= n; i++)
        {
            firstCard.Add(i);
        }
        resultDeck.Add(firstCard);

        // 次の n 枚 (0 を固定し、列ごとに増やしていく)
        for (int i = 1; i <= n; i++)
        {
            List<int> card = new() { 0 };
            for (int j = 1; j <= n; j++)
            {
                card.Add(i + j * n);
            }
            resultDeck.Add(card);
        }

        // 残りの n^2 枚 (y = ax + b の形)
        for (int a = 1; a <= n; a++)
        {
            for (int b = 1; b <= n; b++)
            {
                List<int> card = new() { a };
                for (int x = 1; x <= n; x++)
                {
                    int symbol = (a * x + b) % n;
                    symbol = n + symbol * n + x;
                    card.Add(symbol);
                }
                resultDeck.Add(card);
            }
        }
        return resultDeck;
    }

    /// <summary>
    /// 手札位置に必要な設定データ
    /// </summary>
    [Serializable]
    public struct HandPosConfig
    {
        [Tooltip("手札の中心位置")]
        public Vector3 handPos;
        [Tooltip("手札の半径")]
        public float handRadius;
        [Tooltip("手札の最大角度")]
        public float handMaxAngle;
        [Tooltip("カード間の隙間"), Range(1, 5)]
        public float gapSizeMagnification;
    }
    /// <summary>
    /// 手札位置を計算する
    /// </summary>
    public static (List<Vector3>, List<Quaternion>) CalculateHandPos(int cardCount, HandPosConfig c)
    {
        List<Vector3> positions = new();
        List<Quaternion> rotations = new();

        if (cardCount <= 0) return (positions, rotations);

        float startAngle = -c.handMaxAngle * 0.5f;
        float angleStep = (cardCount > 1) ? c.handMaxAngle / (cardCount - 1) : 0;
        Vector3 center = c.handPos - new Vector3(0, c.handRadius, 0);

        if (cardCount == 1)
        {
            Vector3 position = center + new Vector3(0 ,c.handRadius, 0);
            positions.Add(position);

            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            rotations.Add(rotation);

            return (positions, rotations);
        }

        float gapSize = cardCount * c.gapSizeMagnification * 0.1f;

        for (int i = 0; i < cardCount; i++)
        {
            float angle = startAngle + angleStep * i;
            float radian = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(radian) * c.handRadius * gapSize;
            float y = Mathf.Cos(radian) * c.handRadius;

            Vector3 position = center + new Vector3(x, y, -i * 0.01f);
            positions.Add(position);

            Quaternion rotation = Quaternion.Euler(0, 0, -angle);
            rotations.Add(rotation);
        }
        return (positions, rotations);
    }

}
