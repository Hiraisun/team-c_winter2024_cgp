using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UnitThorn : UnitActionBase
{
    [SerializeField, Tooltip("反撃ダメージ")]
    private float thornDamage = 3;

    private DamageInfo thornDamageInfo;

    void Start()
    {
        thornDamageInfo = new DamageInfo
        {
            attacker = UnitBase,
            damage = thornDamage,
            damageType = DamageType.THORN,
            knockbackDamage = 0
        };

        UnitBase.Events.AddOnDamageReceivedListener(OnDamageReceived);
    }

    private void OnDamageReceived(DamageInfo damageInfo)
    {
        // 近接なら反撃
        if(damageInfo.damageType == DamageType.MELEE)
        {
            damageInfo.attacker.Damage(thornDamageInfo);
        }
    }

    public void SetThornDamage(float damage)
    {
        if(damage < 0)
        {
            Debug.LogError("ダメージが負の値");
            return;
        }
        thornDamage = damage;
        thornDamageInfo.damage = damage;
    }

}
