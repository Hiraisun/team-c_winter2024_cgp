using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using DG.Tweening;


/// <summary>
/// ユニットのノックバック処理
/// 被ダメージが一定以上になるたびにノックバックが発生
/// </summary>
public class UnitKnockback : UnitActionBase
{
    [Header("ノックバック設定")]
    [SerializeField, Tooltip("ノックバック値の累積閾値")]
    private float knockbackThreshold = 100;
    [SerializeField, Tooltip("ノックバック距離")]
    private float knockbackDistance = 1.0f;

    private float recievedKnockbackDamage = 0; //累積ノックバック値

    private CancellationTokenSource cts;
    private CancellationToken ct;


    // 被ダメージ時
    protected override void OnDamageRecieved(DamageInfo damageInfo)
    {
        recievedKnockbackDamage += damageInfo.knockbackDamage;

        // ノックバック閾値に達した
        if(recievedKnockbackDamage >= knockbackThreshold)
        {
            recievedKnockbackDamage = 0;
            if(unitBase.InterruptAction()) // 行動割込み成功
            {
                // ノックバック処理
                cts = new();
                ct = cts.Token;
                KnockbackTask(ct).Forget();
            }
        }
    }

    async UniTask KnockbackTask(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested(); //キャンセルチェック

        unitBase.StartAction(this); //アクション開始を宣言

        Sequence sequence = DOTween.Sequence()
            .Append(transform.DOMoveX(knockbackDistance * unitBase.direction, 0.5f).SetRelative());

        // ノックバック行動をawait
        await sequence.ToUniTask(cancellationToken: ct);

        unitBase.FinishAction(this); //アクション終了を宣言
    }


    // 割り込み処理
    public override bool InterruptAction()
    {
        cts.Cancel();
        cts.Dispose();
        return true;
    }

    // Destroy時処理
    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}
