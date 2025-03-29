using TMPro;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// 説明用ウィンドウの挙動を扱う
/// </summary>
public class UISymbolDescription : MonoBehaviour
{
    // 説明用ウィンドウ
    private GameObject descriptionWindow => this.gameObject;

    // 説明用TMP
    private TextMeshProUGUI descriptionText;

    // キャンセルトークン
    private CancellationTokenSource cts;

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

        Card[] cardCmps = cardManager.cardCmps;

        // シンボルの情報を取得
        foreach(Card card in cardCmps)
        {
            int i = 0;
            foreach(UISymbol symbol in card.CardSymbolCmps)
            {
                symbol.Initialize(card.Symbols[i], cardManager);
                symbol.AddSymbolMouseEnterListener(OnMouseEnterListener);
                symbol.AddSymbolMouseExitListener(OnMouseExitListener);
                i++;
            }
        }
    }
    
    /// <summary>
    /// マウスのホバー(入)のハンドラ
    /// </summary>
    private void OnMouseEnterListener(string description)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();

        descriptionText.text = description;

        descriptionWindow.SetActive(true);

        FollowCursor(cts.Token).Forget();
    }

    /// <summary>
    /// マウスのホバー(出)のハンドラ
    /// </summary>
    private void OnMouseExitListener()
    {
        cts?.Cancel();

        // 非表示にする
        descriptionWindow.SetActive(false);
    }

    // カーソルにウィンドウを追従させる
    private async UniTaskVoid FollowCursor(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            descriptionWindow.transform.position = Input.mousePosition + new Vector3(200, 0 , 0);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }
}
