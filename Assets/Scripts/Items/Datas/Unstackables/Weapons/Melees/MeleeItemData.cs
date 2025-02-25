using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class MeleeItemData : WeaponItemData
{
    [Header("Melee")]
    [SerializeField] float m_baseDamage;
    [SerializeField] int m_baseMaxHits = 1;
    [SerializeField] float m_baseWeight;
    [SerializeField] Sound m_slashSound, m_hitSound;
    public float baseDamage => m_baseDamage;
    public int baseMaxHits => m_baseMaxHits;
    public float baseWeight => m_baseWeight;
    public Sound slashSound => m_slashSound;
    public Sound hitSound => m_hitSound;
}
public abstract class MeleeItem : WeaponItem, IDamageModifyable, IStatDisplayed
{
    new public Melee weapon => base.weapon as Melee;
    new public readonly MeleeItemData data;
    public MeleeItem(MeleeItemData data) : base(data)
    {
        this.data = data;
        damage = data.baseDamage;
        maxHits = data.baseMaxHits;
        weight = data.baseWeight;
    }
    public float damage { get; set; }
    public int maxHits;
    public float weight;

    string prevName;
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        weapon.hitbox.onTriggerStay += OnTriggerStay;
        wielder.animation.onSlashStart += OnSlashStart;
        wielder.animation.onSlashEnd += OnSlashEnd;
        wielder.movement.moveSpeed -= weight;
    }
    public Action<MeleeSlash, MeleeGiveDamageInfo> modifySlashDamageGiven;

    public Action<MeleeSlash> onSlashStart, onSlashEnd;
    public Action<MeleeSlash, GameObject> onSlashHit;
    public Action<MeleeSlash, DamageReceivedInfo> onSlashDamageGive;
    public override void OnWieldUpdate()
    {
        base.OnWieldUpdate();
        if(Input.GetMouseButtonDown(0) && UIScanner.ScanUI().Count <= 0)
        {
            wielder.animation.TriggerSlash();
        }
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        if (currentSlash != null)
        {
            onSlashEnd?.Invoke(currentSlash);
            currentSlash = null;
        }
        weapon.hitbox.onTriggerStay -= OnTriggerStay;
        wielder.animation.onSlashStart -= OnSlashStart;
        wielder.animation.onSlashEnd -= OnSlashEnd;
        wielder.movement.moveSpeed += weight;
    }
    MeleeSlash currentSlash = null;
    List<Collider> checkedColliders = new();
    void OnSlashStart(int comboIndex)
    {
        if(currentSlash != null)
        {
            onSlashEnd?.Invoke(currentSlash);
        }
        checkedColliders.Clear();
        currentSlash = new MeleeSlash()
        {
            comboIndex = comboIndex,
            info = new()
            {
                damage = damage * wielder.statistics.damageMultiplier,
                maxHits = maxHits
            }
        };
        AudioManager.Instance.PlaySound(data.slashSound);
        onSlashStart?.Invoke(currentSlash);
    }
    void OnSlashEnd(int comboIndex)
    {
        if (currentSlash != null && currentSlash.comboIndex == comboIndex)
        {
            onSlashEnd?.Invoke(currentSlash);
            currentSlash = null;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (currentSlash == null || wielder == null) return;
        if (currentSlash.hitCount >= currentSlash.info.maxHits) return;

        if (checkedColliders.Contains(other)) return;
        checkedColliders.Add(other);

        GameObject hitObject = (other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject);
        if(hitObject.TryGetComponent(out Point point))
        {
            if (currentSlash.hit.Contains(point.pointOf)) return;
            else currentSlash.hit.Add(point.pointOf);
        }
        else
        {
            if (currentSlash.hit.Contains(hitObject)) return;
            else currentSlash.hit.Add(hitObject);
        }
        onSlashHit?.Invoke(currentSlash, hitObject);
        if (hitObject.TryGetComponent(out IDamagable tmp))
        {
            MeleeGiveDamageInfo info = new()
            {
                damage = currentSlash.info.damage * wielder.statistics.damageMultiplier,
                hitObject = hitObject
            };
            modifySlashDamageGiven?.Invoke(currentSlash, info);
            DamageReceivedInfo received = tmp.GetDamage(new()
            {
                amount = info.damage,
                source = wielder
            });
            onSlashDamageGive?.Invoke(currentSlash, received);
            currentSlash.hitCount++;
            AudioManager.Instance.PlaySound(data.hitSound);
        }
    }
    public virtual IEnumerable<LangText> GetStats()
    {
        yield return new()
        {
            en = $"Damage: {damage}",
            kr = $"대미지: {damage}"
        };
        yield return new()
        {
            en = $"Max Hits: {maxHits}",
            kr = $"광역 히트 수: {maxHits}"
        };
        yield return new()
        {
            en = $"Weight: {weight}",
            kr = $"무게: {weight}"
        };
    }
}
public class MeleeSlashInfo
{
    public float damage;
    public int maxHits;
}
public class MeleeSlash
{
    public int comboIndex;

    public MeleeSlashInfo info;
    public int hitCount = 0;
    public List<GameObject> hit = new();
}
public class MeleeGiveDamageInfo
{
    public DamageInfo damage;
    public GameObject hitObject;
}
[System.Serializable]
public struct MeleeComboElement
{
    [SerializeField] float m_baseDamage;

    [SerializeField] float m_hitboxStartTime;
    [SerializeField] float m_hitboxEndTime;

    [SerializeField] float m_nextComboStartTime;
    [SerializeField] float m_endTime;
}