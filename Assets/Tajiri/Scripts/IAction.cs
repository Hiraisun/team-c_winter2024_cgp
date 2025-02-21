using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    void Execute();
}

public abstract class ActionScriptableObject : ScriptableObject, IAction
{
    public abstract void Execute();
}

/// <summary>
/// ユニットを生成する
/// </summary>
[CreateAssetMenu(fileName = "SpawnUnitAction", menuName = "ScriptableObject/Actions/SpawnUnit")]
public class SpawnUnitAction : ActionScriptableObject
{
    [SerializeField, Header("生成するユニットのPrefab")]
    private GameObject unitPrefab;

    [SerializeField, Header("生成される座標")]
    private Transform spawnPoint;

    public override void Execute()
    {
        Debug.Log(unitPrefab.name + "を生成した！");
    }
}