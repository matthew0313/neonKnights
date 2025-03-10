using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Energizing", menuName = "Scriptables/Quirks/Multiple/Energizing", order = 0)]
public class QuirkData_Energizing : QuirkData
{
    [Header("Energizing")]
    [SerializeField] float m_minStaminaBonus;
    [SerializeField] float m_maxStaminaBonus;
    public float minStaminaBonus => m_minStaminaBonus;
    public float maxStaminaBonus => m_maxStaminaBonus;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && (target is WeaponItem || target is EquipmentItem);
    }
    public override Quirk Create()
    {
        return new Quirk_Energizing(this);
    }
}
public class Quirk_Energizing : Quirk
{
    new public readonly QuirkData_Energizing data;
    public Quirk_Energizing(QuirkData_Energizing data) : base(data)
    {
        this.data = data;
    }
    float staminaBonus = 0.0f;
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        if(target is WeaponItem weapon)
        {
            weapon.onWield += AddStamina;
            weapon.onUnwield += RemoveStamina;
            if(weapon.wielder != null) AddStamina(weapon.wielder);
        }
        else if(target is EquipmentItem equipment)
        {
            equipment.onEquip += AddStamina;
            equipment.onUnequip += RemoveStamina;
            if(equipment.equipper != null) AddStamina(equipment.equipper);
        }
    }
    void AddStamina(Player player)
    {
        player.movement.maxStamina += staminaBonus;
    }
    void RemoveStamina(Player player)
    {
        player.movement.maxStamina -= staminaBonus;
    }
    public override void SetRandom()
    {
        staminaBonus = Mathf.Round(UnityEngine.Random.Range(data.minStaminaBonus, data.maxStaminaBonus));
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"+{staminaBonus} Stamina",
            kr = $"+{staminaBonus} 스태미나"
        };
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats["staminaBonusPercentage"] = staminaBonus;
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        data.floats.TryGetValue("staminaBonusPercentage", out staminaBonus);
    }
    public override Quirk Copy()
    {
        Quirk_Energizing tmp = base.Copy() as Quirk_Energizing;
        tmp.staminaBonus = staminaBonus;
        return tmp;
    }
}