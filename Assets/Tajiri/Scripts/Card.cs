using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardState
{
    Init,       // 初期状態,未使用
    Hand,       // 手札(非選択)
    Selected,   // 選択中
    Used        // 使用済み, 演出が終了次第破棄
}


public class Card : MonoBehaviour
{
    // SerializeField-----------------------------------------

    [SerializeField, Header("シンボルを表示するSpriteRenderer")]
    private SpriteRenderer[] sr;

    [SerializeField]
    private CardManager cm;

    // プロパティ----------------------------------------------

    public int cardNum{ get; private set; } // カードID

    public List<int> symbolIndices{ get; private set; } // シンボルIDリスト
    public List<SymbolData> symbolsData{ get; private set; } // シンボルデータリスト
    
    public CardState cardState{ get; private set; } // カードの状態

    /// <summary>
    /// 初期化
    /// インスタンス化時に必ず呼ぶ 初期化処理はここで完結
    /// </summary>
    public void Initialize(int cardNum, CardManager cardManager)
    {
        cm = cardManager;
        cardState = CardState.Hand;

        // カードIDを指定
        this.cardNum = cardNum;

        // シンボルを取得
        symbolIndices = cm.GetSymbols(cardNum);
        symbolsData = new();
        foreach (int index in symbolIndices)
        {
            symbolsData.Add(cm.GetSymbolData(index));
        }

        InitializeSymbolImage(); // シンボル画像初期化
        InitializeUnitName(); // ユニット名初期化
    }

    // ビジュアル調整系は別classにしてもいいかも。単一責任の原則!!
    // SymbolのSpriteをImageに適用
    private void InitializeSymbolImage()
    {
        for (int i = 0; i < symbolsData.Count; i++)
        {
            sr[i].sprite = symbolsData[i].symbolSprite;
        }
    }
    // シンボルの右にユニット名を表示する??
    private void InitializeUnitName()
    {
        // TODO:未実装
    }



    // このオブジェクトがクリックされたとき
    private void OnMouseDown()
    {
        // CMに通知
        cm.NotifyCardClicked(this);

        // State制御などはCardが独断でやらない、CMに任せる
        // 敗北演出中に選択できてしまう等のバグを考慮
    }

    /// <summary>
    /// 手札内での位置調整
    /// </summary>
    public void MoveToHandPos(Vector3 position)
    {
        transform.position = position;

        // TODO:Lerpとか
    }


    // State制御----------------------------------------------
    // 演出などで煩雑になるようなら、Stateパターンに書き換えるといいかも
    // 参考：https://qiita.com/AsahinaKei/items/ce8e5d7bc375af23c719

    /// <summary>
    /// ドロー時など 手札にする
    /// </summary>
    public void ToHand()
    {
        cardState = CardState.Hand;
        
        // TODO:手札位置に持ってくる処理とか
    }

    /// <summary>
    /// カードを選択状態に移行
    /// </summary>
    public void Select(){
        cardState = CardState.Selected;

        // TODO:カードハイライトする
        // TODO:音鳴らす
        // TODO:手札の対応シンボルを光らせる
    }

    /// <summary>
    /// 選択状態をキャンセル
    /// </summary>
    public void Deselect(){
        cardState = CardState.Hand;

        // TODO:色々な演出を解除する
    }

    /// <summary>
    /// カードを使用済みにする
    /// </summary>
    public void Use(){
        cardState = CardState.Used;
        
        // TODO:燃えて塵になって消えるとか?

        Destroy(gameObject);
    }

}




