using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Phase", menuName = "ScriptableObject/PhaseData")]
public class PhaseData : ScriptableObject
{
    public string phaseName;
    [TextArea(3, 10)]
    public string phaseDescription;

    [Serializable]
    public struct CycleAction
    {
        public CardEffectBase cardEffect;
        public float interval;
    }
    public List<CycleAction> cycleActions;


    private CancellationTokenSource cts;


    public void ActivatePhase(){
        cts = new();

        foreach (var cycleAction in cycleActions)
        {
            CycleActionTask(cycleAction, cts.Token).Forget();
        }
    }

    public void TerminatePhase(){
        cts.Cancel();
        cts.Dispose();
    }



    // 無限ループするタスク
    private async UniTask CycleActionTask(CycleAction cycleAction, CancellationToken ct)
    {
        while(true)
        {
            await UniTask.WaitForSeconds(cycleAction.interval, cancellationToken: ct);
            cycleAction.cardEffect.Activate(OwnerType.NPC);
        }
    }
}
