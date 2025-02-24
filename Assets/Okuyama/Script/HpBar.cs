using System.Collections;
using UnityEngine;


/// <summary>
/// HPバーの管理クラス
/// </summary>
public class HpBar : MonoBehaviour
{
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform front;

    [SerializeField] private CanvasGroup canvasGroup; // 透明度の変更用

    [Header("設定")]
    [SerializeField] private float fadeDelay = 2f;    // フェード開始までの遅延時間
    [SerializeField] private float fadeDuration = 1f; // フェードにかける時間


    private float maxHP;
    private Coroutine animationCoroutine; // コルーチン管理用

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(float maxHP, float width = 1f, float height = 0.05f)
    {
        this.maxHP = maxHP;

        // サイズ調整
        canvas.sizeDelta = new Vector2(width, height);
        front.offsetMax = new Vector2(0, 0);

        // 透明にしておく
        canvasGroup.alpha = 0;
    }

    /// <summary>
    /// HPバーの更新
    /// </summary>
    public void UpdateHpBar(float currentHP)
    {
        // バーの長さ更新
        float deltaX = - ( 1 - currentHP / maxHP) * canvas.sizeDelta.x;
        front.offsetMax = new Vector2(deltaX, 0);

        // 既存のアニメーションを停止
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        // 新たにアニメーションを開始
        animationCoroutine = StartCoroutine(HPbarAnimation());
    }


    // 時間をかけて変化するアニメーション実装
    private IEnumerator HPbarAnimation()
    {
        canvasGroup.alpha = 1f; // 不透明に

        yield return new WaitForSeconds(fadeDelay);

        // フェードアウト
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0f, timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
