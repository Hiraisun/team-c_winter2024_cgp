using UnityEngine;
using DG.Tweening;

public class CardIconAnimation : MonoBehaviour
{
    public void Initialize(CardCursorRaycast ccr)
    {
        ccr.AddOnEnterObj(OnMouseEnterListener);
        ccr.AddOnExitObj(OnMouseExitListener);
    }

    private void OnMouseEnterListener(GameObject obj)
    {
        obj.transform.DOScale(Vector2.one * 1.1f, 0.2f);
    }

    private void OnMouseExitListener(GameObject obj)
    {
        obj.transform.DOScale(Vector2.one , 0.2f);
    }
}
