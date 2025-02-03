using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class ThrowableItemData : StackableItemData
{
    [Header("Throwable")]
    [SerializeField] Throwable m_prefab;
    [SerializeField] CooldownSource m_cooldownSource;
    [SerializeField] float m_cooldown;
    [SerializeField] Color m_cooldownBarColor;
    public Throwable prefab => m_prefab;
    public CooldownSource cooldownSource => m_cooldownSource;
    public float cooldown => m_cooldown;
    public Color cooldownBarColor => m_cooldownBarColor;
}
public abstract class ThrowableItem : StackableItem, IBarDisplayed, IReloadDisplayed, IStatDisplayed
{
    new public readonly ThrowableItemData data;
    Throwable m_throwable;
    public Throwable throwable
    {
        get
        {
            if (m_throwable == null)
            {
                m_throwable = MonoBehaviour.Instantiate(data.prefab);
                m_throwable.gameObject.SetActive(false);
            }
            return m_throwable;
        }
    }
    public ThrowableItem(ThrowableItemData data) : base(data)
    {
        this.data = data;
    }
    public override RotateMode rotateMode => RotateMode.MouseLocked;
    public override AnimationType heldAnimation => AnimationType.Throwable;
    public bool barDisplay => data.cooldownSource.isOnCooldown;
    public float barFill => 1.0f - data.cooldownSource.cooldownLeft / data.cooldownSource.startCooldown;
    public Color barColor => data.cooldownBarColor;
    public bool reloading => barDisplay;
    public float reloadProgress => barFill;
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        throwable.transform.SetParent(wielder.inventory.equipAnchor, false);
        throwable.gameObject.SetActive(true);
    }
    public override void OnWieldUpdate()
    {
        base.OnWieldUpdate();
        if (data.cooldownSource.isOnCooldown) return;
        if (Input.GetMouseButtonDown(0) && throwing == null)
        {
            throwing = Throwing();
            ItemManager.Instance.StartCoroutine(throwing);
        }
    }
    const float throwTime = 0.5f;
    IEnumerator throwing = null;
    IEnumerator Throwing()
    {
        wielder.animation.TriggerThrow();
        yield return new WaitForSeconds(throwTime);
        throwing = null;
        OnThrow();
        data.cooldownSource.SetCooldown(data.cooldown);
        wielder.cooldowns.AddCooldown(data.cooldownSource);
        containedSlot.count--;
    }
    protected abstract void OnThrow();
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        throwable.gameObject.SetActive(false);
    }
    public virtual IEnumerable<LangText> GetStats()
    {
        yield return new()
        {
            en = $"Cooldown: {data.cooldown}s",
            kr = $"쿨타임: {data.cooldown}s"
        };
    }
}