using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カード制御でつかうアルゴリズムなどのstaticクラス 生のC#
/// </summary>
/// <param name="symbolPerCard">1枚のカードに書かれているシンボルの数</param>
public static class CardAlgrithms
{
    /// <summary>
    /// Dobbleのルールに沿ったデッキを作成するアルゴリズム
    /// </summary>
    public static List<List<int>> GenerateDobbleCards(int symbolPerCard)
    {
        List<List<int>> resultCards = new();
        int n = symbolPerCard - 1;

        // 1枚目のカード (0, 1, 2, ..., n)
        List<int> firstCard = new();
        for (int i = 0; i <= n; i++)
        {
            firstCard.Add(i);
        }
        resultCards.Add(firstCard);

        // 次の n 枚 (0 を固定し、列ごとに増やしていく)
        for (int i = 1; i <= n; i++)
        {
            List<int> card = new() { 0 };
            for (int j = 1; j <= n; j++)
            {
                card.Add(i + j * n);
            }
            resultCards.Add(card);
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
                resultCards.Add(card);
            }
        }

        return resultCards;
    }


    /// <summary>
    /// 手札に持つ座標の計算アルゴリズム
    /// </summary>
    /// <param name="handCount">手札の枚数</param>
    public static List<Vector3> CalculateHandPosition(int handCount)
    {
        // TODO:ここに手札座標の計算を書く


        // ↓ デバッグ用の仮実装-------------
        List<Vector3> positions = new();
        for (int i = 0; i < handCount; i++)
        {
            positions.Add(new Vector3(i * 3 - 7, 0, 0));
        }
        // --------------------------------

        return positions;
    }

    /// <summary>
    /// リストをシャッフルする
    /// 参考：https://qiita.com/o8que/items/bf07f824b3093e78d97a
    /// </summary>
    public static void Shuffle<T>(this IList<T> list) {
        for (int i = list.Count - 1; i > 0; i--) {
            int j = UnityEngine.Random.Range(0, i + 1);
            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    /// <summary>
    /// 一致シンボルを探す
    /// </summary>
    public static int FindMatchSymbol(Card card1, Card card2)
    {
        List<int> symbolIndices1 = card1.symbolIndices;
        List<int> symbolIndices2 = card2.symbolIndices;

        foreach (int index in symbolIndices1) // カード1の各シンボル
        {
            if (symbolIndices2.Contains(index)) // カード2に含まれていたら -> 一致
            {
                return index;
            }
        }
        throw new Exception("一致シンボルなし! カードID : " + card1.cardNum + " , " + card2.cardNum);
    }
}
