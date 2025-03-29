using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オブジェクトにカーソルがホバーしているか検知する
/// </summary>
public class UIBase : MonoBehaviour
{
    private BoxCollider2D objCollider;   // シンボルの当たり判定
    private bool isInsidePrev = false;

    private void Awake()
    {
        try
        {
            objCollider = this.gameObject.GetComponent<BoxCollider2D>();
        }
        catch
        {
            Debug.LogWarning("UISymbol : " + this.gameObject.name + "のBoxCollider2Dを取得できません。");
        }
    }

    private void Update()
    {
        Bounds bounds = objCollider.bounds;

        Vector2 mousePos = UICursor.Instance.CursorPos;

        bool isInsideCrr = mousePos.x >= bounds.min.x && mousePos.x <= bounds.max.x &&
                    mousePos.y >= bounds.min.y && mousePos.y <= bounds.max.y;

        // カーソルが入ったとき
        if (isInsideCrr && !isInsidePrev)
        {
            OnCursorEnter();
        }

        // カーソルが出たとき
        if (!isInsideCrr && isInsidePrev)
        {
            OnCursorExit();
        }

        isInsidePrev = isInsideCrr;
    }

    /// <summary>
    /// カーソルが入ったときに呼ばれる
    /// </summary>
    protected virtual void OnCursorEnter() {}

    /// <summary>
    /// カーソルが出たときに呼ばれる
    /// </summary>
    protected virtual void OnCursorExit() {}
}
