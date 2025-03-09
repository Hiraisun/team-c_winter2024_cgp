using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class UnitAnimatorControll : UnitActionBase
{
    [SerializeField] protected Animator animator;

    void Start()
    {
        UnitBase.Events.AddOnSummonCompleteListener(OnSummonComplete);
        UnitBase.Events.AddOnAttackStartListener(OnAttackStart);
        UnitBase.Events.AddOnAttackInterruptListener(OnAttackInterrupt);
        UnitBase.Events.AddOnDeathListener(OnDeath);
    }

    // 召喚完了時の処理
    private void OnSummonComplete()
    {
        // 待機を終了、歩き始める
        if (animator != null) animator.SetTrigger("SummonComplete"); //召喚完了アニメーション
    }

    // 攻撃開始時の処理
    private void OnAttackStart()
    {
        // アニメーションの再生
        if (animator != null) animator.SetTrigger("AttackStart"); //攻撃開始アニメーション
    }

    // 攻撃中断時の処理
    private void OnAttackInterrupt()
    {
        // アニメーションの再生
        if (animator != null) animator.SetTrigger("AttackInterrupt"); //攻撃中断アニメーション
    }

    // 死亡時の処理
    private async UniTask OnDeath()
    {
        // アニメーションの再生完了を待つ
        if (animator != null)
        {
            animator.SetTrigger("Death"); //死亡アニメーション
            // アニメーション終了待機
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UnitAnimatorControll))]
public class UnitAnimatorControllEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.HelpBox("Animatorのパラメータ名 ↓↓", MessageType.Info);
        EditorGUILayout.LabelField("召喚完了時Trigger : SummonComplete");
        EditorGUILayout.LabelField("攻撃開始時Trigger : AttackStart");
        EditorGUILayout.LabelField("死亡時Trigger     : Death (Loopしないよう注意)");
    }
}
#endif
