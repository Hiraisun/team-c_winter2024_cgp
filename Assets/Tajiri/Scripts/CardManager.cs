using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // 手札のカードが格納されているリスト
    private List<Card> cards = new();

    // 使用するすべてのカード
    private List<Card> deck = new();

    [SerializeField, Header("最初にドローするカードの枚数")]
    int initialDrawCount = 5;

    [SerializeField, Header("１枚のカードに書かれているシンボルの数")]
    int symbolCountPerCard = 4;

    [SerializeField, Header("シンボルのデータを格納するリスト")]
    private List<SymbolData> allSymbols;

    // 選択中のカードの枚数
    public int selectedCardCount = 0;

    private void Start()
    {

    }

    /// <summary>
    /// デッキの初期化
    /// </summary>
    private void InitializeDeck()
    {
        int n = symbolCountPerCard - 1;

        List<List<int>> cardLists = GenerateDobbleCards(n);

        deck.Clear(); // デッキをクリア

        foreach (var symbols in cardLists)
        {
            deck.Add(CreateCard(symbols));
        }
    }

    /// <summary>
    /// Dobbleのルールに沿ったデッキを作成する
    /// </summary>
    /// <param name="n">カード毎のシンボルの数-1の値</param>
    /// <returns></returns>
    private List<List<int>> GenerateDobbleCards(int n)
    {
        List<List<int>> cards = new();

        // 1枚目のカード (0, 1, 2, ..., n)
        List<int> firstCard = new();
        for (int i = 0; i <= n; i++)
        {
            firstCard.Add(i);
        }
        cards.Add(firstCard);

        // 次の n 枚 (0 を固定し、列ごとに増やしていく)
        for (int i = 1; i <= n; i++)
        {
            List<int> card = new() { 0 };
            for (int j = 1; j <= n; j++)
            {
                card.Add(i + j * n);
            }
            cards.Add(card);
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
                cards.Add(card);
            }
        }

        return cards;
    }

    /// <summary>
    /// 引数に対応するシンボルをカードに対応させる
    /// </summary>
    /// <param name="_symbolIndices">付与するシンボルのインデックス</param>
    /// <returns></returns>
    private Card CreateCard(List<int> _symbolIndices)
    {
        Card card = new();
        card.symbols = new List<SymbolData>();

        foreach(int i in _symbolIndices)
        {
            card.symbols.Add(allSymbols[i]);
        }

        return card;
    }
}
