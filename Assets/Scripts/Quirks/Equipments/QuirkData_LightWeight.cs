using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Lightweight", menuName = "Scriptables/Quirks/Equipments/Lightweight", order = 0)]
public class QuirkData_Lightweight : QuirkData
{
    [Header("Lightweight")]
    [SerializeField] float m_minSpeedBoost;
    [SerializeField] float m_maxSpeedBoost;
    public float minSpeedBoost => m_minSpeedBoost;
    public float maxSpeedBoost => m_maxSpeedBoost;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is EquipmentItem;
    }
    public override Quirk Create()
    {
        return new Quirk_Lightweight(this);
    }
}
public class Quirk_Lightweight : Quirk
{
    new public readonly QuirkData_Lightweight data;
    public Quirk_Lightweight(QuirkData_Lightweight data) : base(data)
    {
        this.data = data;
    }
    float speedBoost;
    bool added = false;
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as EquipmentItem).onEquip += OnEquip;
        (target as EquipmentItem).onUnequip += OnUnequip;
        if ((target as EquipmentItem).equipper != null) OnEquip((target as EquipmentItem).equipper);
    }
    public override void SetRandom()
    {
        base.SetRandom();
        speedBoost = Mathf.Round(UnityEngine.Random.Range(data.minSpeedBoost, data.maxSpeedBoost) * 10.0f) / 10.0f;
    }
    void OnEquip(Player equipper)
    {
        if (added) return;
        added = true;
        equipper.movement.moveSpeed += speedBoost;
    }
    void OnUnequip(Player equipper)
    {
        if (!added) return;
        added = false;
        equipper.movement.moveSpeed -= speedBoost;
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"+{speedBoost} speed when equipped",
            kr = $"장비 시 +{speedBoost} 속도"
        };
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats["speedBoost"] = speedBoost;
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        data.floats.TryGetValue("speedBoost", out speedBoost);
    }
    public override Quirk Copy()
    {
        Quirk_Lightweight tmp = base.Copy() as Quirk_Lightweight;
        tmp.speedBoost = speedBoost;
        return tmp;
    }
}