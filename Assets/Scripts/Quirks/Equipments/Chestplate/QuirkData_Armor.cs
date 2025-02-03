using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Armor", menuName = "Scriptables/Quirks/Equipments/Chestplate/Armor", order = 0)]
public class QuirkData_Armor : QuirkData
{
    [Header("Armor")]
    [SerializeField] float m_minCooldown;
    [SerializeField] float m_maxCooldown;
    [SerializeField] LangText m_armorReplenishText, m_armorUseText;
    [SerializeField] Color m_indicateColor;
    public float minCooldown => m_minCooldown;
    public float maxCooldown => m_maxCooldown;
    public LangText armorReplenishText => m_armorReplenishText;
    public LangText armorUseText => m_armorUseText;
    public Color indicateColor => m_indicateColor;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is ChestplateItem;
    }
    public override Quirk Create()
    {
        return new Quirk_Armor(this);
    }
}
public class Quirk_Armor : Quirk
{
    new public readonly QuirkData_Armor data;
    public Quirk_Armor(QuirkData_Armor data) : base(data)
    {
        this.data = data;
    }
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as ChestplateItem).onEquip += OnEquip;
        (target as ChestplateItem).onEquipUpdate += OnEquipUpdate;
        (target as ChestplateItem).onUnequip += OnUnequip;
        if ((target as ChestplateItem).equipper != null) OnEquip((target as ChestplateItem).equipper);
    }
    float counter = 0.0f;
    float cooldown = 5.0f;
    public override void SetRandom()
    {
        base.SetRandom();
        cooldown = Mathf.Round(UnityEngine.Random.Range(data.minCooldown, data.maxCooldown) * 10.0f) / 10.0f;
    }
    bool charged = true;
    bool added = false;
    void OnEquip(Player equipper)
    {
        if (added) return;
        added = true;
        equipper.hpComp.modifyDamageTaken += ModifyDamage;
    }
    void OnEquipUpdate()
    {
        if (!charged)
        {
            counter += Time.deltaTime;
            if (counter >= cooldown)
            {
                TextIndicator.Indicate((target as ChestplateItem).equipper.transform.position, data.armorReplenishText.text, data.indicateColor);
                charged = true;
                counter = 0.0f;
            }
        }
    }
    void OnUnequip(Player equipper)
    {
        if (!added) return;
        added = false;
        equipper.hpComp.modifyDamageTaken -= ModifyDamage;
    }
    void ModifyDamage(DamageInfoWrap wrap)
    {
        if (charged)
        {
            TextIndicator.Indicate((target as ChestplateItem).equipper.transform.position, data.armorUseText.text, data.indicateColor);
            charged = false;
            wrap.info.amount = 0.0f;
        }
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"Nullifies 1 damage every {cooldown}s",
            kr = $"{cooldown}초마다 대미지 1회 무효화"
        };
    }
}