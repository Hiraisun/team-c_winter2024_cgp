using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unitには必ずアタッチする基本コンポーネント
/// イベントの受け渡しなどを行う。
/// 各種耐性など、受け身な情報はここに実装？
/// </summary>
/// //TODO:イベント管理
public class UnitBase : MonoBehaviour
{
    [SerializeField] public BattleManager battleManager;
    [SerializeField] public StageData stageData;

    public UnitTYPE unitType = UnitTYPE.PLAYER; // 敵か味方か
    public Lane lane = Lane.Ground; // レーン

    // 何らかのアクション中か
    public bool isBusy = false;

    // HP
    [SerializeField] private float MaxHP = 100;
    public float HP { get; private set; }

    // 向き : 移動などで乗算するためのパラメータ 
    // (PLAYERは1, ENEMYは-1)
    public float direction{
        get{
            return unitType == UnitTYPE.PLAYER ? 1 : -1;
        }
    }

    void Start()
    {
        HP = MaxHP;
        battleManager.addUnitList(this);
    }

    // ダメージを受ける
    public void Damage(float damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            Destroy(gameObject);
            battleManager.removeUnitList(this);
        }
    }
}
public enum UnitTYPE
{
    PLAYER,
    ENEMY,
}
