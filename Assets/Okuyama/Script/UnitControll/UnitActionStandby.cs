using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UnitActionStandby : UnitActionBase
{
    private bool trigger;

    void Awake()
    {
        trigger = false;
        UnitBase.BattleManager.AddOnSummonCompleteListener(OnSummonComplete);
        UnitBase.Events.AddOnSummonListener(OnSummon);
    }

    async UniTask OnSummon()
    {
        // 号令まで待つ
        await UniTask.WaitUntil(() => trigger, cancellationToken: this.GetCancellationTokenOnDestroy());
    }

    void OnSummonComplete(OwnerType owner){

        // 味方が召喚完了した
        if(owner == UnitBase.Owner){
            trigger = true;
        }
    }
}
