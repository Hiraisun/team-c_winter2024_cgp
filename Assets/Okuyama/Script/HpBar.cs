using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

/// <summary>
/// HPバーの管理クラス
/// </summary>
public class HpBar : MonoBehaviour
{
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform front;

    [SerializeField] private Image frontImage;

    private float maxHP;

    // TODO: 最大HPの変化に対応

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(float maxHP, float width = 1f, float height = 0.05f)
    {
        this.maxHP = maxHP;

        canvas.sizeDelta = new Vector2(width, height);
        front.offsetMax = new Vector2(0, 0);
    }

    /// <summary>
    /// HPバーの更新
    /// </summary>
    public void UpdateHpBar(float currentHP)
    {
        float deltaX = - ( 1 - currentHP / maxHP) * canvas.sizeDelta.x;
        front.offsetMax = new Vector2(deltaX, 0);
    }


}
