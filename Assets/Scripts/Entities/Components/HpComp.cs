using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpComp : MonoBehaviour, IDamagable, IHealable, IAlimentable
{
    //hp
    [SerializeField] float m_baseHp;
    [SerializeField] float m_maxShield = 0;
    [SerializeField] Transform m_alimentEffectAnchor;
    public Transform alimentEffectAnchor => m_alimentEffectAnchor;
    public float baseHp
    {
        get => m_baseHp;
        set
        {
            m_baseHp = value;
            onHpChange?.Invoke();
        }
    }
    float m_bonusHp = 0;
    public float bonusHp
    {
        get => m_bonusHp;
        set
        {
            m_bonusHp = value;
            onHpChange?.Invoke();
        }
    }
    float m_hp;

    public float maxHp => baseHp + bonusHp;
    public float hp
    {
        get => m_hp;
        set
        {
            m_hp = value;
            onHpChange?.Invoke();
        }
    }

    public float maxShield
    {
        get => m_maxShield;
        set
        {
            m_maxShield = value;
            onHpChange?.Invoke();
        }
    }
    float m_shield;
    public float shield
    {
        get => m_shield;
        set
        {
            m_shield = value;
            onHpChange?.Invoke();
        }
    }
    public bool invincible = false;
    public bool dead { get; private set; } = false;

    public Action<float> onShieldDamage, onDamage, onHeal;
    public Action onHpChange;
    public Action onDeath;
    void Awake()
    {
        hp = maxHp;
        shield = maxShield;
    }
    const float shieldRegenStartTime = 10.0f, shieldRegenPercentagePerSecond = 0.2f;
    float shieldRegenCounter = 0.0f;

    void OnValidate()
    {
        if (Application.isPlaying) onHpChange?.Invoke();
    }
    void Update()
    {
        ShieldRegenUpdate();
        AlimentsUpdate();
    }
    void ShieldRegenUpdate()
    {
        if (dead || maxShield <= 0) return;
        if (shieldRegenCounter < shieldRegenStartTime) shieldRegenCounter += Time.deltaTime;
        else if(shield < maxShield)
        {
            shield = Mathf.Min(maxShield, shield + maxShield * shieldRegenPercentagePerSecond * Time.deltaTime);
        }
    }
    public DamageReceivedInfo GetDamage(float damage)
    {
        if (dead) return new() { receivedDamage = 0 };
        shieldRegenCounter = 0.0f;
        if(shield > 0)
        {
            float amount = Mathf.Min(damage, shield);
            shield -= amount;
            onShieldDamage?.Invoke(amount);
            return new() { receivedDamage = amount };
        }
        else
        {
            float amount = Mathf.Min(damage, hp);
            hp -= amount;
            onDamage?.Invoke(damage);
            if (hp <= 0)
            {
                dead = true;
                onDeath?.Invoke();
                return new() { receivedDamage = amount, fatal = true };
            }
            else
            {
                return new() { receivedDamage = amount };
            }
        }
    }

    public void GetHeal(float heal)
    {
        if (dead) return;
        float amount = Mathf.Min(maxHp - hp, heal);
        hp += amount;
        onHeal?.Invoke(amount);
    }
    public void Revive()
    {
        if (!dead) return;
        dead = false;
        hp = maxHp;
    }
    List<StatusAliment> aliments = new();
    List<StatusAliment> addQueue = new(), removeQueue = new();
    public void GetAliment(StatusAliment aliment)
    {
        addQueue.Add(aliment);
    }
    public void RemoveAliment(StatusAliment aliment)
    {
        removeQueue.Add(aliment);
    }
    public bool FindAliment(Func<StatusAliment, bool> predicate, out StatusAliment aliment)
    {
        foreach(var i in aliments)
        {
            if (predicate.Invoke(i))
            {
                aliment = i;
                return true;
            }
        }
        aliment = null;
        return false;
    }
    public bool FindAliment<T>(out T aliment) where T : StatusAliment
    {
        if(FindAliment((tmp) => tmp is T, out StatusAliment tmp))
        {
            aliment = tmp as T;
            return true;
        }
        else
        {
            aliment = null;
            return false;
        }
    }
    void AlimentsUpdate()
    {
        foreach(var i in addQueue)
        {
            aliments.Add(i);
            i.OnAlimentAdd(this);
        }
        addQueue.Clear();
        foreach (var i in aliments) i.OnAlimentUpdate();
        foreach(var i in removeQueue)
        {
            aliments.Remove(i);
            i.OnAlimentRemove();
        }
        removeQueue.Clear();
    }
}