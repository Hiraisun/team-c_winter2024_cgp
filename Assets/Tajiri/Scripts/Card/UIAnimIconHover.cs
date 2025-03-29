using UnityEngine;
using DG.Tweening;

/// <summary>
/// マウスのカーソルがホバーしているときの処理をつかさどるクラス
/// </summary>
public class UIAnimIconHover : MonoBehaviour
{
    [SerializeField, Header("判定を受けるレイヤー")]
    private LayerMask targetLayer;

    [SerializeField, Header("カードのフレームタグ")]
    private string cardFrameTag = "CardFrame";

    // 前回にRayがHitしたオブジェクト
    private GameObject previousHitObj;

    // マウスカーソルの位置
    private Vector2 mousePos;

    private void Update()
    {
        OnMouseEnterAndExit();
    }

    /// <summary>
    /// カーソルがオブジェクトに触れる際の処理
    /// </summary>
    private void OnMouseEnterAndExit()
    {
        // マウスカーソルの位置を更新
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Rayが貫通したオブジェクト
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero, 0f, targetLayer);
        
        GameObject newHitObj = null;

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag(cardFrameTag))
            {
                newHitObj = hit.collider.gameObject;
                if (newHitObj != previousHitObj)
                {
                    DOScaleOnMouseEnter(newHitObj);
                }
                break;
            }
        }

        if (previousHitObj != null && previousHitObj != newHitObj)
        {
            DOScaleOnMouseExit(previousHitObj);
        }

        previousHitObj = newHitObj;
    }

    // 拡大
    private void DOScaleOnMouseEnter(GameObject obj)
    {
        obj.transform.DOScale(Vector2.one * 1.1f, 0.2f);
    }

    // 縮小
    private void DOScaleOnMouseExit(GameObject obj)
    {
        obj.transform.DOScale(Vector2.one , 0.2f);
    }
}
