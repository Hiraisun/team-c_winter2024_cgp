using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// ユーザー操作の状態
/// 将来的にポーズ中、カード効果の確認中など追加していく
/// </summary>
public enum ControllState{
    None,
    Selected,  //カードを1枚選択中
}

public class CardManager : MonoBehaviour
{
    // SerializeField-----------------------------------------

    [SerializeField, Header("カードのPrefab")]
    private GameObject cardPrefab;

    [SerializeField, Header("シンボルデータを格納するリスト")]
    private List<SymbolData> allSymbols = new();

    [SerializeField, Header("最初にドローするカードの枚数")]
    int initialDrawCount = 5;

    [SerializeField, Header("１枚のカードに書かれているシンボルの数")]
    int symbolCountPerCard = 4;

    // プロパティ----------------------------------------------

    // ユーザー操作の状態
    private ControllState controllState = ControllState.None;

    // 使用するすべてのカード
    private List<List<int>> deck;

    // 手札
    private List<Card> hand = new();

    // 選択中のカード 
    public Card selectedCard; // リストじゃなくした

    // 引く可能性のあるカードID (ちょっと実装が汚い)
    private List<int> drawabkeCardIDs;


    private void Start()
    {
        // デッキを初期化(デッキを生成)
        InitializeDeck();

        // DrawableCardIDsを初期化
        drawabkeCardIDs = Enumerable.Range(0, deck.Count).ToList();

        for(int i = 0; i < initialDrawCount; i++)
        {
            Draw();
        }
    }

    /// <summary>
    /// デッキの初期化
    /// </summary>
    private void InitializeDeck()
    {
        deck = CardAlgrithms.GenerateDobbleCards(symbolCountPerCard);
    }
    

    /// <summary>
    /// デッキからカードを1枚ドローする
    /// </summary>
    public void Draw()
    {
        // TODO:手札がいっぱいとかの処理
        

        // 引くカードIDを決定
        CardAlgrithms.Shuffle(drawabkeCardIDs); //シャッフル
        int randomIndex = drawabkeCardIDs.First(); //先頭を取り出す
        drawabkeCardIDs.RemoveAt(0);
        
        // インスタンス化, 初期化
        GameObject cardObj = Instantiate(cardPrefab, this.transform); //散らかるので一応childに
        Card card = cardObj.GetComponent<Card>();
        card.Initialize(randomIndex, this);

        // 手札に追加
        hand.Add(card);
        card.ToHand();
        AdjustHandPosition(); //位置調整
    }


    /// <summary>
    /// クリックされたことをCardに通知してもらう
    /// ここが全部背負いすぎている。もうちょっと分割したい。ネスト深すぎクソ野郎
    /// </summary>
    public void NotifyCardClicked(Card card)
    {
        if (controllState == ControllState.None) //何も選択していない状態
        {
            if (selectedCard != null) throw new Exception("State:None だが selectedCardが存在する");

            if (card.cardState == CardState.Hand) //手札をクリックした
            {
                SelectCard(card); //選択状態に
            }
        }
        else if (controllState == ControllState.Selected)  //1枚選択中
        {
            if(selectedCard == null) throw new Exception("State:Selected だが selectedCardがnull");

            if (card.cardState == CardState.Hand)  //手札をクリックした
            {
                if(TryUseCard(selectedCard, card)) //カードを使えた
                {
                    selectedCard = null;
                    controllState = ControllState.None;
                }
                else //カードを使えなかった (マナ不足とか?)
                {
                    //TODO:「マナ不足です」的なメッセージとか音とか
                }
            }
            else if (card.cardState == CardState.Selected) //選択中のカードをクリックした
            {
                DeselectCard(); //選択解除
            }
        }

    }


    /// <summary>
    /// 全ての手札を位置調整する
    /// </summary>
    private void AdjustHandPosition()
    {
        List<Vector3> handPositions = CardAlgrithms.CalculateHandPosition(hand.Count);
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].MoveToHandPos(handPositions[i]);
        }
    }


    // ControlState系--------------------------------------------
    /// <summary>
    /// カードを選択
    /// </summary>
    private void SelectCard(Card card)
    {
        card.Select();
        selectedCard = card;
        controllState = ControllState.Selected;
    }
    /// <summary>
    /// カードを選択解除
    /// </summary>
    private void DeselectCard()
    {
        selectedCard.Deselect();
        selectedCard = null;
        controllState = ControllState.None;
    }
    /// <summary>
    /// カードを二枚指定して使用(を検討)
    /// </summary>
    /// <returns>カードを使えたらtrue</returns>
    private bool TryUseCard(Card card1, Card card2)
    {

        // 一致シンボルを探す
        int matchSymbol = CardAlgrithms.FindMatchSymbol(card1, card2);

        // TODO:マナが足りないとかで使えないならfalseを返す

        // シンボルに対応するアクションを実行
        //allSymbols[matchSymbol].action.Execute();
        Debug.Log(allSymbols[matchSymbol].symbolSprite.name + "が呼び出されました！"); //デバッグ用

        // 選択リセット
        controllState = ControllState.None;
        selectedCard = null;

        // 引けるようにする
        drawabkeCardIDs.Add(card1.cardNum);
        drawabkeCardIDs.Add(card2.cardNum);

        // 手札から削除
        hand.Remove(card1);
        hand.Remove(card2);

        // 使用を通知 -> 破棄
        card1.Use();
        card2.Use();

        // 位置調整
        AdjustHandPosition();
        return true;
    }



    // 完璧で究極のGetters----------------------------------------------------
    /// <summary>
    /// カードIDに対応するシンボルを取得
    /// </summary>
    public List<int> GetSymbols(int cardNum){
        // 整合性チェック
        if (cardNum < 0 || cardNum >= deck.Count) {
            Debug.LogError("Invalid card number: " + cardNum);
            return null;
        }
        return deck[cardNum];
    }

    /// <summary>
    /// シンボルIDに対応するSymbolDataを取得
    /// </summary>
    public SymbolData GetSymbolData(int symbolIndex){
        // 整合性チェック
        if (symbolIndex < 0 || symbolIndex >= allSymbols.Count) {
            Debug.LogError("Invalid symbol index: " + symbolIndex);
            return null;
        }
        return allSymbols[symbolIndex];
    }


}
