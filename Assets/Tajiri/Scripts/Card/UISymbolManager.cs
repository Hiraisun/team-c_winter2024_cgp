using TMPro;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// シンボルUIと説明用ウィンドウの管理を行う
/// </summary>
public class UISymbolManager : MonoBehaviour
{
    // 説明用ウィンドウ
    private GameObject descriptionWindow => this.gameObject;

    // 説明用TMP
    private TextMeshProUGUI descriptionText;

    // キャンセルトークン
    private CancellationTokenSource cts;

    // ホバー中になっているシンボルの数
    private int hoverCount = 0;

    /// <summary>
    /// 初期化する
    /// </summary>
    public void Initialize(CardManager cardManager)
    {
        try 
        {
            this.descriptionText = descriptionWindow.GetComponentInChildren<TextMeshProUGUI>();
        }
        catch
        {
            Debug.LogError("CardSymbolUI : 説明用TMPが見つかりませんでした");
        }

        // カードの情報を取得
        Card[] cardCmps = cardManager.cardCmps;

        // シンボルの情報を取得
        foreach(Card card in cardCmps)
        {
            int i = 0;
            // カードの４つのシンボルをそれぞれ初期化
            foreach(UISymbol symbol in card.CardSymbolCmps)
            {
                symbol.Initialize(card.Symbols[i], cardManager, this);
                i++;
            }
        }

        descriptionWindow.SetActive(false);
    } 

    // OnCursorEnter → OnCursorExitの順で実行しないと表示されなくなります。

    /// <summary>
    /// マウスがホバー状態になったときに呼び出す
    /// </summary>
    public void OnCursorEnter(string description)
    {
        // 最初にホバー状態になるとき
        if (hoverCount == 0)
        {
            // 前回の待機を解除
            cts?.Cancel();
            cts = new CancellationTokenSource();

            // ウィンドウを表示
            descriptionWindow.SetActive(true);

            // 解除まで待機
            FollowCursor(cts.Token).Forget();
        }

        // TMPを更新
        descriptionText.text = description;

        hoverCount++;
    }

    /// <summary>
    /// マウスホバー状態解除時に呼び出す
    /// </summary>
    public void OnCursorExit()
    {
        hoverCount--;

        // ホバー中のオブジェクトが０になるとき
        if (hoverCount <= 0)
        {
            // ループを終了
            cts?.Cancel();

            // ウィンドウを非表示
            descriptionWindow.SetActive(false);
        }
    }

    /// <summary>
    /// カーソルにウィンドウを追従させる
    /// </summary>
    private async UniTaskVoid FollowCursor(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // 所定の位置にウィンドウを表示
            descriptionWindow.transform.position = Input.mousePosition + new Vector3(200, 0 , 0);

            // 毎フレーム更新
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }
}
