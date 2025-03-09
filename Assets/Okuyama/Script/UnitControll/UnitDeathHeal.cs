using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UnitDeathHeal : UnitActionBase
{
    [SerializeField, Tooltip("回復量")]
    private float healAmount = 3;
    [SerializeField, Tooltip("範囲")]
    private float range = 2;

    private void Start()
    {
        UnitBase.Events.AddOnDeathListener(OnDeath);
    }

    // 死亡時周囲の味方を回復
    private async UniTask OnDeath()
    {
        // TODO:演出

        await UniTask.WaitForSeconds(0.5f); // 一瞬待つ
        
        float fromX = transform.position.x - range;
        float toX = transform.position.x + range;

        List<UnitBase> allyUnitList = UnitBase.BattleManager.GetAllyUnitList(UnitBase.Owner);

        foreach (var ally in allyUnitList)
        {
            if (IsInRange(fromX, toX, ally))
            {
                ally.Heal(healAmount);
            }
        }
    }

    private bool IsInRange(float fromX, float toX, UnitBase target)
    {
        float targetX = target.transform.position.x;
        
        if (UnitBase.direction == 1) //左向き(味方)
        {
            if (toX <= targetX && targetX <= fromX)
            {
                return true;
            }
        }
        else //右向き(敵)
        {
            if (fromX <= targetX && targetX <= toX)
            {
                return true;
            }
        }
        return false;
    }

    public void SetHealAmount(float amount)
    {
        if(amount < 0)
        {
            Debug.LogError("負の回復量");
            return;
        }
        healAmount = amount;
    }
}
