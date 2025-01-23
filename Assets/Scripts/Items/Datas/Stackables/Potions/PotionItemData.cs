using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class PotionItemData : StackableItemData
{
    [Header("Potion")]
    [SerializeField] Potion m_prefab;
    [SerializeField] float m_drinkTime;
    [SerializeField] float m_cooldown;
    [SerializeField] CooldownSource m_cooldownSource;
    [SerializeField] Color m_cooldownBarColor;

    public Potion prefab => m_prefab;
    public float drinkTime => m_drinkTime;
    public float cooldown => m_cooldown;
    public CooldownSource cooldownSource => m_cooldownSource;
    public Color cooldownBarColor => m_cooldownBarColor;
}
public abstract class PotionItem : Item, IBarDisplayed, IStatDisplayed, IReloadDisplayed
{
    new public readonly PotionItemData data;
    Potion m_potion;
    public Potion potion
    {
        get
        {
            if (m_potion == null)
            {
                m_potion = MonoBehaviour.Instantiate(data.prefab);
                m_potion.gameObject.SetActive(false);
            }
            return m_potion;
        }
    }
    public PotionItem(PotionItemData data) : base(data)
    {
        this.data = data;
    }
    public override AnimationType heldAnimation => AnimationType.Potion;
    public bool barDisplay => data.cooldownSource.isOnCooldown;
    public float barFill => 1.0f - data.cooldownSource.cooldownLeft / data.cooldownSource.startCooldown;
    public Color barColor => data.cooldownBarColor;
    public bool reloading => barDisplay;
    public float reloadProgress => barFill;
    IEnumerator drinking = null;
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        potion.transform.SetParent(wielder.inventory.equipAnchor, false);
        potion.gameObject.SetActive(true);
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
        data.cooldownSource.SetCooldown(data.cooldown);
        wielder.cooldowns.AddCooldown(data.cooldownSource);
        containedSlot.count--;
    }
    protected abstract void OnDrink();
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        if(drinking != null)
        {
            ItemManager.Instance.StopCoroutine(drinking);
        }
        potion.gameObject.SetActive(false);
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
