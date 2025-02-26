using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


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
    [SerializeField] private float fadeDelay = 3f;    // フェード開始までの遅延
    [SerializeField] private float fadeDuration = 0.5f; // フェード時間

    
    private float maxHP;
    private Color defaultColor; // 初期色

    private Sequence animationSequence; //演出sequence

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

        animationSequence = DOTween.Sequence()
            .Append(frontImage.DOColor(defaultColor, flushDuration)) // フラッシュ演出
            .AppendInterval(fadeDelay) //待機
            .Append(canvasGroup.DOFade(0, fadeDuration)) // フェードアウト
            .SetAutoKill(false).Pause().SetLink(gameObject);
    }

    /// <summary>
    /// HPバーの更新
    /// </summary>
    public void UpdateHpBar(float currentHP)
    {
        // バーの長さ更新
        float deltaX = - ( 1 - currentHP / maxHP) * canvas.sizeDelta.x;
        front.offsetMax = new Vector2(deltaX, 0);

        //演出
        AnimationStart();
    }

    private void AnimationStart()
    {
        frontImage.color = Color.white;
        canvasGroup.alpha = 1;
        animationSequence.Restart();
    }
}

