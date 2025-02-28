using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのリソースを管理する
/// </summary>
public class PlayerResourceManager : MonoBehaviour
{
    [SerializeField]
    protected CardManager cardManager;

    [SerializeField]
    protected float maxMana = 10; // 最大マナ
    public float MaxMana { get => maxMana; }

    protected float mana; // 現在のマナ
    public float Mana { get => mana; }

    protected Action OnManaChanged;
    public void AddManaChangedListener(Action listener) => OnManaChanged += listener;

    /// <summary>
    /// マナを消費する。 成功したらtrue, 失敗したらfalseを返す
    /// </summary>
    public bool ConsumeMana(float cost)
    {
        if (cost < 0) {
            Debug.LogError("ConsumeMana: マナ消費量が負です");
            return false;
        }else if (cost > mana) {
            Debug.Log("ConsumeMana: マナが足りない");
            return false;
        }else{
            mana -= cost;
            OnManaChanged?.Invoke();
            return true; //成功
        }
    }
}
