using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 敵への範囲攻撃処理
/// </summary>
[RequireComponent(typeof(UnitBase))] // UnitBase必須やで
public class UnitAttackAllEnemy : MonoBehaviour
{
    [SerializeField] private UnitBase unitBase;

    [Serializable]
    public class LaneRange
    {
        public Lane lane;
        public float range;
    }

    // 攻撃範囲リスト (各レーン)
    [SerializeField] private List<LaneRange> attackRangeList;

    void OnValidate()
    {
        // 自動アタッチ
        if(!Application.isPlaying)
        {
            unitBase = GetComponent<UnitBase>();
        }
    }

    //TODO:攻撃クールダウン

    void Update()
    {
        if(!unitBase.isBusy)
        {
            if (IsInRange())
            {
                unitBase.isBusy = true;
                //TODO:実際の攻撃処理
                //TODO:攻撃アニメーション
            }
        }
    }

    // 攻撃対象が範囲内に存在するか
    private bool IsInRange()
    {
        //ターゲット候補
        List<UnitBase> targetList = unitBase.battleManager.getEnemyUnitList(unitBase.unitType);
        
        //各候補について
        foreach (var target in targetList)
        {
            //レーンごとにチェック
            foreach (var laneRange in attackRangeList)
            {
                if (target.lane == laneRange.lane)
                {
                    float fromX = transform.position.x;
                    float toX = fromX + laneRange.range * unitBase.direction * -1;
                    float targetX = target.transform.position.x;

                    fromX *= unitBase.direction;
                    toX *= unitBase.direction;
                    targetX *= unitBase.direction;

                    if (toX <= targetX && targetX <= fromX)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    

    void OnDrawGizmosSelected()
    {
        //攻撃範囲の描画
        Gizmos.color = Color.red;
        foreach (var laneRange in attackRangeList)
        {
            float Y = unitBase.stageData.LaneParams.Find(x => x.lane == laneRange.lane).PosY + 0.55f;
            Vector3 Center = new Vector3(transform.position.x + laneRange.range * unitBase.direction * -0.5f, Y, 0);
            Vector3 Size = new Vector3(laneRange.range, 1, 1);
            Gizmos.DrawWireCube(Center, Size);
        }
    }
}
