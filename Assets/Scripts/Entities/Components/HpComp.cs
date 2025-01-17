using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

    public Action<DamageReceivedInfo> onDamage;
    public Action<float> onHeal;
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
    public DamageReceivedInfo GetDamage(DamageInfo damage)
    {
        Debug.Log("RRRRRRAWRADWDAWDDWA");
        if (dead) return new() { receivedDamage = 0 };
        shieldRegenCounter = 0.0f;
        float amountLeft = damage.amount;
        DamageReceivedInfo info = new();

        float tmp;
        if(!damage.ignoreShield && shield > 0)
        {
            tmp = Mathf.Min(amountLeft, shield);
            shield -= tmp;
            amountLeft -= tmp;
            info.receivedShieldDamage += tmp;
        }
        tmp = Mathf.Min(amountLeft, hp);
        hp -= tmp;
        info.receivedDamage += tmp;
        if (hp <= 0)
        {
            dead = true;
            onDeath?.Invoke();
            info.fatal = true;
        }
        onDamage?.Invoke(info);
        return info;
    }

    public virtual void GetHeal(float heal)
    {
        if (dead) return;
        float amount = Mathf.Min(maxHp - hp, heal);
        hp += amount;
        onHeal?.Invoke(amount);
    }
    public virtual void Revive()
    {
        if (!dead) return;
        dead = false;
        hp = maxHp;
    }
    List<StatusAliment> aliments = new();
    List<StatusAliment> addQueue = new(), removeQueue = new();
    public void ApplyAliment(StatusAliment aliment)
    {
        aliment.AddTo(this);
    }
    public virtual void AddAliment(StatusAliment aliment)
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
            i.OnAlimentAdd();
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
#if UNITY_EDITOR
[CustomEditor(typeof(HpComp))]
public class HpComp_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Deal Damage")) (target as HpComp).GetDamage(10.0f);
    }
}
#endif