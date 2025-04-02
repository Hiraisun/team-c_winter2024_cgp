using UnityEngine;

/// <summary>
/// オブジェクトにカーソルがホバーしているか検知する。
/// </summary>
public class UIBase : MonoBehaviour
{
    private SpriteRenderer objSpriteRenderer;   // シンボルの当たり判定
    private bool isInsidePrev = false;

    private void Awake()
    {
        try
        {
            objSpriteRenderer = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        }
        catch
        {
            Debug.LogWarning("UISymbol : " + this.gameObject.name + "のSpriteRendererを取得できません。");
        }
    }

    private void Update()
    {
        // 境界情報を更新
        Bounds bounds = objSpriteRenderer.bounds;

        // カーソルの位置
        Vector2 mousePos = UICursor.Instance.CursorPos;

        // 境界内(X, Yのみ)にカーソルが存在すれば真
        bool isInsideCrr = mousePos.x >= bounds.min.x && mousePos.x <= bounds.max.x &&
                    mousePos.y >= bounds.min.y && mousePos.y <= bounds.max.y;

        // カーソルが入ったとき
        if (isInsideCrr && !isInsidePrev) OnCursorEnter();

        // カーソルが出たとき
        if (!isInsideCrr && isInsidePrev) OnCursorExit();

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
