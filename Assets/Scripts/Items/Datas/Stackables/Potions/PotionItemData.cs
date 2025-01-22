using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class PotionItemData : StackableItemData
{
    [Header("Potion")]
    [SerializeField] Potion m_potion;
    [SerializeField] float m_drinkTime;
    [SerializeField] float m_cooldown;
    [SerializeField] CooldownSource m_cooldownSource;
    [SerializeField] Color m_cooldownBarColor;

    public Potion potion => m_potion;
    public float drinkTime => m_drinkTime;
    public float cooldown => m_cooldown;
    public CooldownSource cooldownSource => m_cooldownSource;
    public Color cooldownBarColor => m_cooldownBarColor;
}
public abstract class PotionItem : Item, IBarDisplayed, IStatDisplayed
{
    new PotionItemData data;
    public PotionItem(PotionItemData data) : base(data)
    {
        this.data = data;
    }

    public bool barDisplay => data.cooldownSource.isOnCooldown;
    public float barFill => 1.0f - data.cooldownSource.cooldownLeft / data.cooldown;
    public Color barColor => data.cooldownBarColor;
    IEnumerator drinking = null;
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        wielder.animation.SetAnimationType(AnimationType.Potion);
    }
    public override void OnWieldUpdate()
    {
        base.OnWieldUpdate();
        if (data.cooldownSource.isOnCooldown) return;
        if (Input.GetMouseButtonDown(0) && drinking == null)
        {
            drinking = Drinking();
            ItemManager.Instance.StartCoroutine(drinking);
        }
    }
    IEnumerator Drinking()
    {
        wielder.animation.TriggerDrinkStart();
        yield return new WaitForSeconds(data.drinkTime);
        wielder.animation.TriggerDrinkEnd();
        drinking = null;
        OnDrink();
        data.cooldownSource.cooldownLeft = data.cooldown;
        wielder.cooldowns.AddCooldown(data.cooldownSource);
        containedSlot.count--;
    }
    protected abstract void OnDrink();
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        wielder.animation.SetAnimationType(AnimationType.None);
        if(drinking != null)
        {
            ItemManager.Instance.StopCoroutine(drinking);
        }
    }
    public virtual IEnumerable<LangText> GetStats()
    {
        yield return new()
        {
            en = $"Cooldown: {data.cooldown}",
            kr = $"쿨타임: {data.cooldown}"
        };
    }
    
}
