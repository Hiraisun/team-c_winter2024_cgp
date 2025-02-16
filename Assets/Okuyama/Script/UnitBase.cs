
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

// 基本的なユニットオブジェクトを扱うクラス
public class UnitBase : MonoBehaviour
{
    public enum UnitType{
        Ally,
        Enemy,
        None
    }
    [SerializeField] private UnitType unitType;  //敵か味方か

    public enum Lane{
        Ground,
        Sky,
    }
    [SerializeField] private Lane lane;  //レーン
    
    [Header("Status")]
    [SerializeField] private int hp; //体力
    [SerializeField] private int atk; //攻撃力
    [SerializeField] private float speed; //移動速度

    [System.Serializable]
    public struct AttackRange{
        public bool Available;
        public float start;
        public float end;
    }
    [SerializeField] private AttackRange attackRangeSky;
    [SerializeField] private AttackRange attackRangeGround;


    
    public enum State{
        Move,
        Attack,
        Dead,
    }
    private State state = State.Move; //状態

    // 味方は1、敵は-1 となる変数。移動などに乗算
    private int direction
    {
        get
        {
            return (unitType == UnitType.Ally) ? 1 : -1;
        }
    }  
    
    public void Start()
    {

    }


    public void Update()
    {
        

        switch (state)
        {
            case State.Move:
                transform.position -= new Vector3(speed * Time.deltaTime * direction, 0, 0);


                break;
            case State.Attack:


                break;
            case State.Dead:


                break;
        }
        
    }





    void OnDrawGizmosSelected()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;

        //state表示
        UnityEditor.Handles.Label(transform.position + new Vector3(0,1,0), state.ToString(),style);

        //攻撃範囲表示
        //地上
        if (attackRangeGround.Available)
        {
            Vector3 Center = new Vector3(transform.position.x - (attackRangeGround.start + attackRangeGround.end) * 0.5f * direction, 
                                            BattleManager.instance.LaneY[Lane.Ground], 0);
            Vector3 Size = new Vector3(attackRangeGround.end - attackRangeGround.start, 0.3f, 0.1f);
            Gizmos.DrawCube(Center, Size);
        }
        //空中
        if (attackRangeSky.Available)
        {
            Vector3 Center = new Vector3(transform.position.x - (attackRangeSky.start + attackRangeSky.end) * 0.5f * direction, 
                                            BattleManager.instance.LaneY[Lane.Sky], 0);
            Vector3 Size = new Vector3(attackRangeSky.end - attackRangeSky.start, 0.3f, 0.1f);
            Gizmos.DrawCube(Center, Size);
        }

    }
}
