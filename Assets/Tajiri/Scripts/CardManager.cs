using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // 使用するすべてのカード
    [SerializeField]
    private List<List<int>> deck;

    // 手札
    [SerializeField]
    private List<int> hand;

    // 手札のゲームオブジェクト
    [SerializeField]
    private List<GameObject> cards;

    [SerializeField, Header("カードのPrefab")]
    private GameObject cardPrefab;

    [SerializeField, Header("最初にドローするカードの枚数")]
    int initialDrawCount = 5;

    [SerializeField, Header("１枚のカードに書かれているシンボルの数")]
    int symbolCountPerCard = 4;

    [SerializeField, Header("シンボルのデータを格納するリスト")]
    private List<SymbolData> allSymbols = new();

    // 選択中のカード
    [SerializeField]
    public List<Card> selectedCards;

    private void Start()
    {
        // デッキを初期化(デッキを生成)
        InitializeDeck();

        for(int i = 0; i <= initialDrawCount - 1; i++)
        {
            GameObject card = Instantiate(cardPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            cards.Add(card);
            hand.Add(-1);
            Draw(i);
        }
    }

    /// <summary>
    /// デッキの初期化
    /// </summary>
    private void InitializeDeck()
    {
        int n = symbolCountPerCard - 1;

        deck = GenerateDobbleCards(n);
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
    /// カードが選択されたときに実行されるスクリプト. ２枚選ばれた状態の時に作用する.
    /// </summary>
    public void UseCard()
    {
        // 選択されたカードが2枚以下なら実行しない
        if (selectedCards.Count != 2) return;

        // 選択されたカードのシンボルのインデックスのリスト
        List<int> combinedSymbolIndices = new();

        // 結合されたリストから２回登場するシンボルを特定
        foreach(Card card in selectedCards)
        {
            card.isSelected = false;
            foreach(int index in card.symbolIndices)
            {
                // indexがすでにリストに含まれていたら処理を中断
                if (combinedSymbolIndices.Contains(index))
                {
                    // デバッグ用
                    Debug.Log(allSymbols[index].symbolSprite.name + "が呼び出されました！");

                    // indexに対応するシンボルに対応するアクションを実行
                    //allSymbols[index].action.Execute();

                    // ループを終了
                    break;
                }
                // indexが初めて出てきたものならリストに追加
                combinedSymbolIndices.Add(index);
            }
        }

        // 2枚のカードのトラッシュ
        List<int> selectedCardIndices = Trash();

        // 2枚ドロー
        Draw(selectedCardIndices[0]);
        Draw(selectedCardIndices[1]);
    }

    /// <summary>
    /// 選択されたカードを記録しトラッシュする
    /// </summary>
    List<int> Trash()
    {
        List<int> cardsToRemove = new();
        List<int> selectedCardIndices = new();

        foreach (var _card in selectedCards)
        {
            cardsToRemove.Add(_card.cardNum);
        }

        foreach (var card in cardsToRemove)
        {
            selectedCardIndices.Add(hand.IndexOf(card));
            hand.Remove(card);
        }

        selectedCards.Clear();

        return selectedCardIndices;
    }


    /// <summary>
    /// デッキからカードを1枚ドローする
    /// </summary>
    private void Draw(int index)
    {
        int randomIndex;

        while (true)
        {
            randomIndex = Random.Range(0, deck.Count);
            if (!hand.Contains(randomIndex)) break;
        }

        hand[index] = randomIndex;

        Card card = cards[index].GetComponent<Card>();
        card.symbolIndices = deck[hand[index]];
        card.cardNum = hand[index];
        card.symbols.Clear();

        for (int j = 0; j <= deck[hand[index]].Count - 1; j++)
        {
            card.symbols.Add(allSymbols[deck[hand[index]][j]]);
        }
        card.ApplySymbols();
    }
}
