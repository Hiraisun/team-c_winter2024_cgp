using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class UnitAnimatorControll : UnitActionBase
{
    [SerializeField] protected Animator animator;

    void Start()
    {
        unitBase.Events.AddOnDeathListener(OnDeath);
        unitBase.Events.AddOnAttackStartListener(OnAttackStart);
    }

    // 攻撃開始時の処理
    private void OnAttackStart()
    {
        // アニメーションの再生
        if (animator != null) animator.SetTrigger("AttackStart"); //攻撃開始アニメーション
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

[CustomEditor(typeof(UnitAnimatorControll))]
public class UnitAnimatorControllEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.HelpBox("Animatorのパラメータ名 ↓↓", MessageType.Info);
        EditorGUILayout.LabelField("攻撃開始時Trigger : AttackStart");
        EditorGUILayout.LabelField("死亡時Trigger     : Death (Loopしないよう注意)");
    }
}
