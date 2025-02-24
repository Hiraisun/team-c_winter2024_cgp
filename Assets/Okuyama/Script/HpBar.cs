using System.Collections;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// HPバーの管理クラス
/// </summary>
public class HpBar : MonoBehaviour
{
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform front;

    [SerializeField] private CanvasGroup canvasGroup; // 透明度の変更用
    [SerializeField] private Image frontImage; // HPバーの色変更用

    [Header("設定")]
    [SerializeField] private float flushDuration = 0.2f; // フラッシュ演出にかける時間
    [SerializeField] private float fadeStart = 3f;    // フェード開始までの遅延
    [SerializeField] private float fadeDuration = 0.5f; // フェード時間

    
    private float maxHP;
    private Coroutine animationCoroutine; // コルーチン管理用

    [SerializeField] Color defaultColor; // 初期色

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
        defaultColor = frontImage.color;
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
        float timer = Time.time;

        canvasGroup.alpha = 1f; // 不透明に
        frontImage.color = Color.white; // フラッシュ色に変更

        // フラッシュを徐々に解除
        while (Time.time - timer < flushDuration)
        {
            frontImage.color = Color.Lerp(Color.white, defaultColor, (Time.time - timer) / flushDuration);
            yield return null;
        }
        frontImage.color = defaultColor; // 初期色に戻す

        // フェードアウト開始まで待機
        while (Time.time - timer < fadeStart) yield return null;

        // フェードアウト
        timer += fadeStart;
        while (Time.time - timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0f, (Time.time - timer) / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
