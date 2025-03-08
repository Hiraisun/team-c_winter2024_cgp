using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    void Start()
    {
        if (animator != null)
        {
            // 最初のアニメーションクリップの長さを取得
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                float delay = clipInfo[0].clip.length;
                Destroy(gameObject, delay);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}