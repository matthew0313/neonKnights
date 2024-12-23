using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpComp : MonoBehaviour, IDamagable, IHealable
{
    [SerializeField] float m_maxHp;
    public float maxHp
    {
        get { return m_maxHp; }
        set
        {
            m_maxHp = value;
            onHpChange?.Invoke();
        }
    }
    public float hp { get; private set; }

    public Action<float> onDamage, onHeal;
    public Action onHpChange;
    public Action onDeath;
    bool dead = false;
    public void GetDamage(float damage)
    {
        if (dead) return;
        float amount = Mathf.Min(hp, damage);
        hp -= amount;
        onDamage?.Invoke(amount);
        onHpChange?.Invoke();
        if(hp <= 0)
        {
            dead = true;
            onDeath?.Invoke();
        }
    }

    public void GetHeal(float heal)
    {
        if (dead) return;
        float amount = Mathf.Min(maxHp - hp, heal);
        hp += amount;
        onHeal?.Invoke(amount);
        onHpChange?.Invoke();
    }
}