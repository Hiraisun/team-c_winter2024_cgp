using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public partial class UnitBase{

    /// <summary>
    /// イベント管理用のクラス
    /// </summary>
    public class UnitEvents
    {
        // 初期化時
        private event Func<UniTask> OnSummon;
        public void AddOnSummonListener(Func<UniTask> listener) => OnSummon += listener;
        public async UniTask InvokeSummon()
        {
            if (OnSummon != null)
            {
                await UniTask.WhenAll(OnSummon.GetInvocationList().Cast<Func<UniTask>>().Select(d => d.Invoke()));
            }
        }

        // 初期化完了時
        private Action OnSummonComplete;
        public void AddOnSummonCompleteListener(Action listener) => OnSummonComplete += listener;
        public void InvokeSummonComplete() => OnSummonComplete?.Invoke();

        // 攻撃開始時 攻撃コンポーネントが発火する
        private Action OnAttackStart;
        public void AddOnAttackStartListener(Action listener) => OnAttackStart += listener;
        public void InvokeAttackStart() => OnAttackStart?.Invoke();

        // 与ダメージ時
        private Action<UnitBase> OnDamageDealt;
        public void AddOnDamageDealtListener(Action<UnitBase> listener) => OnDamageDealt += listener;
        public void InvokeDamageDealt(UnitBase target) => OnDamageDealt?.Invoke(target);
        
        // 被ダメージ時
        private Action<DamageInfo> OnDamageReceived;
        public void AddOnDamageReceivedListener(Action<DamageInfo> listener) => OnDamageReceived += listener;
        public void InvokeDamageReceived(DamageInfo damageInfo) => OnDamageReceived?.Invoke(damageInfo);

        // 死亡時
        private event Func<UniTask> OnDeath;
        public void AddOnDeathListener(Func<UniTask> listener) => OnDeath += listener;
        public async UniTask InvokeDeath()
        {
            if (OnDeath != null)
            {
                await UniTask.WhenAll(OnDeath.GetInvocationList().Cast<Func<UniTask>>().Select(d => d.Invoke()));
            }
        }
    }
    public UnitEvents Events { get; private set; } = new UnitEvents();

    
}
