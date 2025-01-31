using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class HpComp : MonoBehaviour, IDamagable, IHealable, IAlimentable, IDamageTakenMultiplierAdjustable
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
            if (shield > maxShield) shield = maxShield;
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

    public Action<DamageReceivedInfo> onDamage;
    public Action<float> onHeal;
    public Action onHpChange;
    public Action onDeath, onRevive;
    public Action onAlimentUpdate;
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
        else if (shield < maxShield)
        {
            shield = Mathf.Min(maxShield, shield + maxShield * shieldRegenPercentagePerSecond * Time.deltaTime);
        }
    }
    public float damageTakenMultiplier { get; set; } = 1.0f;
    public virtual DamageReceivedInfo GetDamage(DamageInfo damage)
    {
        if (dead) return new() { receivedDamage = 0 };
        shieldRegenCounter = 0.0f;
        float amountLeft = damage.amount * damageTakenMultiplier;
        DamageReceivedInfo info = new();

        float tmp;
        if (!damage.ignoreShield && shield > 0)
        {
            tmp = Mathf.Min(amountLeft, shield);
            shield -= tmp;
            amountLeft -= tmp;
            info.receivedShieldDamage += tmp;
        }
        tmp = Mathf.Min(amountLeft, hp);
        hp -= tmp;
        info.receivedDamage += tmp;
        bool dying = false;
        if (hp <= 0)
        {
            info.fatal = true;
            dying = true;
        }
        info.isWeakPointHit = damage.isWeakPointHit;
        info.isResistPointHit = damage.isResistPointHit;
        onDamage?.Invoke(info);
        if (dying) Die();
        return info;
    }

    public virtual void GetHeal(float heal)
    {
        if (dead) return;
        float amount = Mathf.Min(maxHp - hp, heal);
        hp += amount;
        heal -= amount;
        float amount2 = Mathf.Min(maxShield - shield, heal);
        shield += amount2;
        onHeal?.Invoke(amount + amount2);
    }
    public void Die()
    {
        if (dead) return;
        dead = true;
        OnDeath();
    }
    protected virtual void OnDeath()
    {
        onDeath?.Invoke();
    }
    public virtual void Revive()
    {
        if (!dead) return;
        dead = false;
        OnRevive();
    }
    protected virtual void OnRevive()
    {
        hp = maxHp;
        shield = maxShield;
    }
    List<StatusAliment> aliments = new();
    List<StatusAliment> addQueue = new(), removeQueue = new();
    public virtual void ApplyAliment(StatusAliment aliment)
    {
        aliment.AddTo(this);
    }
    public void AddAliment(StatusAliment aliment)
    {
        addQueue.Add(aliment);
    }
    public void RemoveAliment(StatusAliment aliment)
    {
        removeQueue.Add(aliment);
    }
    public IEnumerable<StatusAliment> GetAliments()
    {
        foreach (var i in aliments) yield return i;
    }
    public bool FindAliment(Func<StatusAliment, bool> predicate, out StatusAliment found)
    {
        foreach(var i in aliments)
        {
            if (predicate(i))
            {
                found = i;
                return true;
            }
        }
        foreach(var i in addQueue)
        {
            if (predicate(i))
            {
                found = i;
                return true;
            }
        }
        found = null;
        return false;
    }
    public bool FindAliment<T>(out T found) where T : StatusAliment
    {
        if (FindAliment((tmp) => tmp is T, out StatusAliment tmp2))
        {
            found = tmp2 as T;
            return true;
        }
        found = null;
        return false;
    }
    void AlimentsUpdate()
    {
        bool updated = false;
        if(addQueue.Count > 0)
        {
            updated = true;
            foreach (var i in addQueue)
            {
                aliments.Add(i);
                i.OnAlimentAdd();
            }
            addQueue.Clear();
        }
        if(removeQueue.Count > 0)
        {
            updated = true;
            foreach (var i in removeQueue)
            {
                aliments.Remove(i);
                i.OnAlimentRemove();
            }
            removeQueue.Clear();
        }
        foreach (var i in aliments) i.OnAlimentUpdate();
        if (updated) onAlimentUpdate?.Invoke();
    }
    public HpCompSaveData Save()
    {
        HpCompSaveData data = new();
        data.hp = hp;
        data.shield = shield;
        foreach(var i in aliments)
        {
            DataUnit tmp = new();
            tmp.integers["type"] = (int)i.type;
            i.Save(tmp);
            data.aliments.Add(tmp);
        }
        return data;
    }
    public void Load(HpCompSaveData data)
    {
        hp = data.hp;
        shield = data.shield;
        foreach(var i in data.aliments)
        {
            StatusAliment tmp = StatusAliment.GetAliment((AlimentType)i.integers["type"]);
            tmp.Load(i);
            tmp.AddTo(this);
        }
    }
}
[System.Serializable]
public class HpCompSaveData
{
    public float hp, shield;
    public List<DataUnit> aliments = new();
}