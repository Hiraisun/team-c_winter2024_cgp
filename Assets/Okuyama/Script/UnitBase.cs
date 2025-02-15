
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
    public UnitType unitType;  //敵か味方か

    public enum Lane{
        Sky,
        Ground,
    }
    public Lane lane;  //レーン

    
    
    
    
    public void Update()
    {
        
    }
}
