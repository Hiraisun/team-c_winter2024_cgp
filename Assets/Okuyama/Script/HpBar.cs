using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private RectTransform canvas;

    [SerializeField] private RectTransform middle;
    [SerializeField] private RectTransform front;

    private float maxHP;
    private float currentHP;

    public void Initialize(float maxHP, float width = 1f, float height = 0.05f)
    {
        canvas.sizeDelta = new Vector2(width, height);
        this.maxHP = maxHP;
        currentHP = maxHP;
    }

    public void UpdateHealth(float currentHP)
    {
        this.currentHP = currentHP;

        float deltaX = - ( 1 - currentHP / maxHP) * canvas.sizeDelta.x;

        front.offsetMax = new Vector2(deltaX, 0);
    }


}
