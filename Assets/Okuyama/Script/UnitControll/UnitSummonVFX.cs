using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UnitSummonVFX : UnitActionBase
{
    void Awake()
    {
        unitBase.AddOnSummonListener(OnSummon);
    }

    async UniTask OnSummon()
    {
        Debug.Log("召喚エフェクト再生");
        // 召喚エフェクト再生
        // エフェクト再生完了後にアクション終了
        await UniTask.Delay(1000);
    }
}
