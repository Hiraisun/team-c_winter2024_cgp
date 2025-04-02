using TMPro;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Security;

/// <summary>
/// シンボルUIと説明用ウィンドウの管理を行う
/// </summary>
public class UISymbolManager : MonoBehaviour
{
    [SerializeField, Header("カーソルとウィンドウ間の距離")]
    private Vector2 gap;

    // y軸に対象なgap
    private Vector2 inversionGap;

    // 説明用ウィンドウ
    private GameObject descriptionWindow => this.gameObject;

    // 説明用TMP
    private TextMeshProUGUI descriptionText;

    // キャンセルトークン
    private CancellationTokenSource cts;

    // ホバー中になっているシンボルの数
    private int hoverCount = 0;

    // 画面の幅
    private float screenWidth;

    /// <summary>
    /// 初期化する
    /// </summary>
    public void Initialize(CardManager cardManager)
    {
        // 説明用文を表示するTMPを取得
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

        // y軸に反転したgapを設定
        inversionGap = new Vector2(-gap.x, gap.y);

        // スクリーンの幅を設定
        screenWidth = Screen.width;

        // ウィンドウを非表示
        descriptionWindow.SetActive(false);
    } 

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
            descriptionWindow.transform.position = CalcWindowPos();

            // 毎フレーム更新
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    private Vector2 CalcWindowPos()
    {
        // カーソルの位置
        Vector2 mousePos = Input.mousePosition;

        // ウィンドウの表示位置
        Vector2 windowPos;

        // スクリーンの左側にカーソルがあるとき
        if (mousePos.x < screenWidth / 2) windowPos = mousePos + gap;

        // スクリーンの右側にカーソルがあるとき
        else windowPos = mousePos + inversionGap;

        return windowPos;
    }
}
