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
/// ���j�b�g�𐶐�����
/// </summary>
[CreateAssetMenu(fileName = "SpawnUnitAction", menuName = "ScriptableObject/Actions/SpawnUnit")]
public class SpawnUnitAction : ActionScriptableObject
{
    [SerializeField, Header("�������郆�j�b�g��Prefab")]
    private GameObject unitPrefab;

    [SerializeField, Header("�����������W")]
    private Transform spawnPoint;

    public override void Execute()
    {
        Debug.Log(unitPrefab.name + "�𐶐������I");
    }
}