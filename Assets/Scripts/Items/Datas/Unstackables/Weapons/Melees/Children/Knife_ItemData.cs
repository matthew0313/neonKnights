using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Knife", menuName = "Scriptables/Items/Unstackables/Weapons/Melees/Knife", order = 0)]
public class Knife_ItemData : MeleeItemData
{
    [Header("Curse")]
    [SerializeField] float m_curseDamageMultiplier;
    [SerializeField] float m_curseSelfDamage;
    public float curseDamageMultiplier => m_curseDamageMultiplier;
    public float curseSelfDamage => m_curseSelfDamage;
    public override Item Create()
    {
        return new Knife_Item(this);
    }
}
public class Knife_Item : MeleeItem, ICurseable
{
    new public readonly Knife_ItemData data;
    new public Knife weapon => base.weapon as Knife;
    public Knife_Item(Knife_ItemData data) : base(data)
    {
        this.data = data;
    }
    public override AnimationType heldAnimation => AnimationType.Knife;
    bool m_cursed = false;
    public bool cursed
    {
        get => m_cursed;
        set
        {
            if (m_cursed == value) return;
            if(m_cursed == false)
            {
                damage *= data.curseDamageMultiplier / 100.0f;
                onSlashDamageGive += Curse_OnSlashDamageGive;
            }
            else
            {
                damage /= data.curseDamageMultiplier / 100.0f;
                onSlashDamageGive -= Curse_OnSlashDamageGive;
            }
            m_cursed = value;
            weapon.SetCurse(value);
            UpdateModel();
        }
    }
    void Curse_OnSlashDamageGive(MeleeSlash slash, DamageReceivedInfo info)
    {
        wielder.hpComp.GetDamage(data.curseSelfDamage);
    }
    public LangText curseEffectDesc => new()
    {
        en = $"+{data.curseDamageMultiplier}% damage, {data.curseSelfDamage} self damage",
        kr = $"+{data.curseDamageMultiplier}% 대미지, {data.curseSelfDamage} 자가 대미지"
    };
    public override ItemModel GetModel()
    {
        Knife_Model tmp = base.GetModel() as Knife_Model;
        tmp.SetCurse(cursed);
        return tmp;
    }
    public override Item Copy()
    {
        Knife_Item tmp = base.Copy() as Knife_Item;
        tmp.cursed = cursed;
        return tmp;
    }
}