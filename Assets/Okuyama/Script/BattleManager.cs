using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

// ユニットの制御、戦闘関連を扱うマネージャークラス
// ひとまずリアルタイム制を採用。
public class BattleManager : MonoBehaviour
{
    // シングルトン(簡易)
    public static BattleManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }



    // 盤面に存在するユニットのリスト
    public List<UnitBase> unitList = new List<UnitBase>();

    void Update()
    {
        unitList[0].unitType = UnitBase.UnitType.Ally;
    }


}
