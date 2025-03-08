using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UnitSummonVFX : UnitActionBase
{
    void Awake()
    {
        unitBase.Events.AddOnSummonListener(OnSummon);
    }

    async UniTask OnSummon()
    {
        // TODO:召喚エフェクト再生
        await UniTask.Delay(2000);
    }
}
