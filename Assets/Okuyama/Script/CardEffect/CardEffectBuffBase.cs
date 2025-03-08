using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 次の召喚を強化するカード効果
/// </summary>
public abstract class CardEffectBuffBase : CardEffectBase
{
    [SerializeField, Tooltip("演出の色")]
    Color buffColor = Color.white;
    public Color BuffColor { get { return buffColor; } }

    /// <summary>
    /// カード効果 : BattleManagerにバフを予約させる
    /// </summary>
    protected override void CardEffect(OwnerType owner)
    {
        // バフ予約
        battleManager.ReserveBuff(this, owner);
    }

    /// <summary>
    /// バフの効果処理
    /// Unitbase.Initialize() の前に呼ばれる。パラメータ変更などは機能しないので注意。
    /// </summary>
    public abstract void Buff(GameObject unitObject, UnitBase unitBase);
}
