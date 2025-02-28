using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カード効果(及びNPCの行動効果)の基底クラス ScriptableObjectとして実装
/// </summary>
public abstract class CardEffectBase : ScriptableObject
{
    protected BattleManager battleManager;

    [SerializeField, Tooltip("発動コスト (未実装, 定義のみ)")] 
    private float manaCost = 0;
    public float ManaCost { get { return manaCost; } }
    
    /// <summary>
    /// カード効果の発動
    /// </summary>
    public void Activate(OwnerType owner)
    {
        // 初回実行時に参照を保存
        if (battleManager == null) battleManager = FindAnyObjectByType<BattleManager>();
        if (battleManager == null) {
            Debug.LogWarning("CardEffect: BattleManagerが見つかりませんでした");
            return;
        }

        //効果処理
        Effect(owner);
    }


    /// <summary>
    /// 効果処理
    /// </summary>
    protected abstract void Effect(OwnerType owner);
}