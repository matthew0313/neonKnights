using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Shielding", menuName = "Scriptables/Quirks/Multiple/Shielding", order = 0)]
public class QuirkData_Shielding : QuirkData
{
    [Header("Shielding")]
    [SerializeField] float m_minShieldBonus;
    [SerializeField] float m_maxShieldBonus;
    [SerializeField] float m_upgradeShieldBonusIncrease;
    public float minShieldBonus => m_minShieldBonus;
    public float maxShieldBonus => m_maxShieldBonus;
    public float upgradeShieldBonusIncrease => m_upgradeShieldBonusIncrease;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && (target is WeaponItem || target is EquipmentItem);
    }
    public override Quirk Create()
    {
        return new Quirk_Shielding(this);
    }
}
public class Quirk_Shielding : Quirk
{
    new public readonly QuirkData_Shielding data;
    public Quirk_Shielding(QuirkData_Shielding data) : base(data)
    {
        this.data = data;
    }
    float shieldBonus = 0.0f;
    bool added = false;
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        if(target is EquipmentItem equipment)
        {
            equipment.onEquip += AddShield;
            equipment.onUnequip += RemoveShield;
            if(equipment.equipper != null) AddShield((target as EquipmentItem).equipper);
        }
        else if(target is WeaponItem weapon)
        {
            weapon.onWield += AddShield;
            weapon.onUnwield += RemoveShield;
            if (weapon.wielder != null) AddShield((target as Item).wielder);
        }
    }
    void AddShield(Player wielder)
    {
        if (added) return;
        added = true;
        wielder.hpComp.maxShield += shieldBonus;
    }
    void RemoveShield(Player wielder)
    {
        if (!added) return;
        added = false;
        wielder.hpComp.maxShield -= shieldBonus;
    }
    public override void SetRandom()
    {
        shieldBonus = Mathf.Round(UnityEngine.Random.Range(data.minShieldBonus, data.maxShieldBonus));
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"+{shieldBonus} shield when {(target is EquipmentItem ? "equipped" : "held")}",
            kr = $"{(target is EquipmentItem ? "입고" : "들고")} 있으면 +{shieldBonus} 실드"
        };
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats["shieldBonus"] = shieldBonus;
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        data.floats.TryGetValue("shieldBonus", out shieldBonus);
    }
    public override Quirk Copy()
    {
        Quirk_Shielding tmp = base.Copy() as Quirk_Shielding;
        tmp.shieldBonus = shieldBonus;
        return tmp;
    }
    public override bool IsUpgradable()
    {
        return shieldBonus < data.maxShieldBonus;
    }
    public override void Upgrade()
    {
        base.Upgrade();
        shieldBonus = Mathf.Min(shieldBonus + data.upgradeShieldBonusIncrease, data.maxShieldBonus);
    }
}