
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 基本的なユニットオブジェクトを扱うクラス
public class UnitBase : MonoBehaviour
{
    public enum UnitType{
        Ally,
        Enemy,
        None
    }
    private UnitType unitType;  //敵か味方か

    public enum Lane{
        Ground,
        Sky,
    }
    private Lane lane;  //レーン
    
    private int hp; //体力
    private int atk; //攻撃力
    private float speed; //移動速度
    
    
    public void Update()
    {
        
    }
}
