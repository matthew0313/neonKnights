using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Diamond Ring", menuName = "Scriptables/Items/Unstackables/Equipments/Accessories/Diamond Ring", order = 0)]
public class DiamondRing_ItemData : AccessoryItemData
{
    [Header("DiamondRing")]
    [SerializeField] float m_shieldBoost;
    [SerializeField] float m_speedBoost;

    [Header("Curse")]
    [SerializeField] float m_curseAttackBoostPercentage;
    [SerializeField] float m_curseHPDrain;
    public float shieldBoost => m_shieldBoost;
    public float speedBoost => m_speedBoost;
    public float curseAttackBoostPercentage => m_curseAttackBoostPercentage;
    public float curseHPDrain => m_curseHPDrain;
    public override Item Create()
    {
        return new DiamondRing_Item(this);
    }
}
public class DiamondRing_Item : AccessoryItem, ICurseable
{
    new public readonly DiamondRing_ItemData data;

    

    public DiamondRing_Item(DiamondRing_ItemData data) : base(data)
    {
        this.data = data;
    }
    bool m_cursed = false;
    public bool cursed
    {
        get => m_cursed;
        set
        {
            if (m_cursed == value) return;
            if (m_cursed == false)
            {
                onEquip += Curse_OnEquip;
                onEquipUpdate += Curse_OnEquipUpdate;
                onUnequip += Curse_OnUnequip;
                if (equipper != null) Curse_OnEquip(equipper);
            }
            else
            {
                onEquip -= Curse_OnEquip;
                onEquipUpdate -= Curse_OnEquipUpdate;
                onUnequip -= Curse_OnUnequip;
            }
            m_cursed = value;
            UpdateModel();
        }
    }
    public LangText curseEffectDesc => new LangText()
    {
        en = $"+{data.curseAttackBoostPercentage}% Damage, -{data.curseHPDrain} HP every second",
        kr = $"대미지 +{data.curseAttackBoostPercentage}, 초마다 -{data.curseHPDrain} HP"
    };
    public override void OnEquip(Player equipper)
    {
        base.OnEquip(equipper);
        equipper.movement.moveSpeed += data.speedBoost;
        equipper.hpComp.maxShield += data.shieldBoost;
    }
    public override void OnUnequip(Player equipper)
    {
        base.OnUnequip(equipper);
        equipper.movement.moveSpeed -= data.speedBoost;
        equipper.hpComp.maxShield -= data.shieldBoost;
    }
    void Curse_OnEquip(Player equipper)
    {
        equipper.statistics.damageMultiplier *= (1.0f + data.curseAttackBoostPercentage / 100.0f);
    }
    float counter = 0.0f;
    void Curse_OnEquipUpdate()
    {
        counter += Time.deltaTime;
        if(counter >= 1.0f)
        {
            counter = 0.0f;
            equipper.hpComp.GetDamage(data.curseHPDrain);
        }
    }
    void Curse_OnUnequip(Player equipper)
    {
        equipper.statistics.damageMultiplier /= (1.0f + data.curseAttackBoostPercentage / 100.0f);
    }
    public override ItemModel GetModel()
    {
        DiamondRing_Model tmp = base.GetModel() as DiamondRing_Model;
        tmp.SetCurse(cursed);
        return tmp;
    }

    public override IEnumerable<LangText> GetStats()
    {
        yield return new()
        {
            en = $"Shield Bonus: {data.shieldBoost}",
            kr = $"실드 증가량: {data.shieldBoost}"
        };
        yield return new()
        {
            en = $"Speed Bonus: {data.speedBoost}",
            kr = $"속도 증가량: {data.speedBoost}"
        };
    }
}