using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// グローバルなサウンド再生
/// </summary>
public class AudioObserver : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    [Space(10)]
    [SerializeField] private CardManager cardManager;

    [Space(10)]
    [SerializeField] private AudioClip cardDrawSE;
    [SerializeField] private AudioClip cardSelectSE;
    [SerializeField] private AudioClip cardDeselectSE;
    [SerializeField] private AudioClip summonUnitSE;
    [SerializeField] private AudioClip useFailedSE;


    private void Start()
    {
        cardManager.AddCardDrawnListener(() => PlaySE(cardDrawSE));
        cardManager.AddCardSelectedListener((Card card) => PlaySE(cardSelectSE));
        cardManager.AddCardDeselectedListener(() => PlaySE(cardDeselectSE));
        cardManager.AddCardUsedListener(() => PlaySE(summonUnitSE));
        cardManager.AddCardUseFailedListener(() => PlaySE(useFailedSE));
    }



    /// <summary>
    /// SE再生
    /// </summary>
    private void PlaySE(AudioClip se)
    {
        if(se == null){
            Debug.LogWarning("SE未設定");
            return;
        }
        seSource.PlayOneShot(se);
    }


}
