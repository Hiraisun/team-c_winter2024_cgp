using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// マウスのカーソルがホバーしている
/// </summary>
public class CardCursorRaycast : MonoBehaviour
{
    [SerializeField, Header("判定を受けるレイヤー")]
    private LayerMask targetLayer;

    [SerializeField, Header("カードのフレームタグ")]
    private string cardFrameTag = "CardFrame";

    [SerializeField]
    private CardIconAnimation cardIconAnimation;

    private event Action<GameObject> OnEnterObj;
    public void AddOnEnterObj(Action<GameObject> listener) => OnEnterObj += listener;

    private event Action<GameObject> OnExitObj;
    public void AddOnExitObj(Action<GameObject> listener) => OnExitObj += listener;

    private GameObject previousHitObj;

    private Vector2 mousePos;

    private void Awake()
    {
        cardIconAnimation.Initialize(this);
    }

    private void Update()
    {
        OnMouseEnterAndExit();
    }

    private void OnMouseEnterAndExit()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero, 0f, targetLayer);

        GameObject newHitObj = null;

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag(cardFrameTag))
            {
                newHitObj = hit.collider.gameObject;
                if (newHitObj != previousHitObj)
                {
                    OnEnterObj?.Invoke(newHitObj);
                }
                break;
            }
        }

        if (previousHitObj != null && previousHitObj != newHitObj)
        {
            OnExitObj?.Invoke(previousHitObj);
        }

        previousHitObj = newHitObj;
    }

}
