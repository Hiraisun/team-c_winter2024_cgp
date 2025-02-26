using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    [SerializeField]
    private List<PhaseData> phaseDatas;

    private CancellationTokenSource cts;

    void Start()
    {
        cts = new();
        PhaseLoop(cts.Token).Forget();
    }


    private async UniTask PhaseLoop(CancellationToken ct)
    {
        while(true)
        {
            PhaseData phaseData = phaseDatas[Random.Range(0, phaseDatas.Count)];
            Debug.Log($"Phase Start: {phaseData.phaseName}");
            phaseData.ActivatePhase();

            await UniTask.Delay(Random.Range(30, 60) * 1000, cancellationToken: ct);

            phaseData.TerminatePhase();
        }
    }
}
