using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ManaBar : MonoBehaviour
{
    [SerializeField]
    private RectTransform parent;
    [SerializeField]
    private RectTransform frontBar;
    [SerializeField]
    private TextMeshProUGUI manaText;

    [Space(10)]
    [SerializeField]
    private PlayerResourceManager playerResourceManager;


    void Awake()
    {
        playerResourceManager.AddManaChangedListener(HandleUpdateMana);
    }

    /// <summary>
    /// マナ残量の更新時
    /// </summary>
    void HandleUpdateMana()
    {
        float ratio = playerResourceManager.Mana / playerResourceManager.MaxMana;

        // バーの長さ更新
        float width = ratio * parent.sizeDelta.x;
        frontBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        // テキスト表示の更新
        int mana = (int)playerResourceManager.Mana;
        int maxMana = (int)playerResourceManager.MaxMana;
        manaText.text = $"{mana,2} / {maxMana,2}";
        
    }
}
