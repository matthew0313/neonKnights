using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.VisualScripting.Member;

public abstract class GunItemData : WeaponItemData
{
    [Header("Default Stats")]
    [SerializeField] bool m_auto;
    [SerializeField] float m_baseDamage;
    [SerializeField] float m_baseFireRate, m_baseRange, m_baseSpread;
    [SerializeField] int m_baseMagSize, m_baseBulletCount, m_baseShotCount;
    [SerializeField] Bullet m_defaultBullet;
    [SerializeField] float m_baseBulletSpeed;
    [SerializeField] int m_defaultPierce = 0;
    public bool auto => m_auto;
    public float baseDamage => m_baseDamage;
    public float baseFireRate => m_baseFireRate;
    public float baseRange => m_baseRange;
    public float baseSpread => m_baseSpread;
    public int baseMagSize => m_baseMagSize;
    public int baseBulletCount => m_baseBulletCount;
    public int baseShotCount => m_baseShotCount;
    public Bullet defaultBullet => m_defaultBullet;
    public float baseBulletSpeed => m_baseBulletSpeed;
    public int defaultPierce => m_defaultPierce;
}
public abstract class GunItem : WeaponItem, INumberDisplayed
{
    new public readonly GunItemData data;
    new public readonly Gun weapon;
    public GunItem(GunItemData data) : base(data)
    {
        this.data = data;
        weapon = base.weapon as Gun;
        auto = data.auto;
        damage = data.baseDamage;
        fireRate = data.baseFireRate;
        range = data.baseRange;
        spread = data.baseSpread;
        magSize = data.baseMagSize;
        bulletCount = data.baseBulletCount;
        bullets = bulletCount;
        bullet = data.defaultBullet;
        bulletSpeed = data.baseBulletSpeed;
        pierce = data.defaultPierce;
        shotCount = data.baseShotCount;
    }
    //stats
    public bool auto;
    public float damage;
    public float fireRate;
    public float range;
    public float spread;
    public int magSize;
    public int bulletCount;
    public Bullet bullet;
    public float bulletSpeed;
    public int pierce = 0;
    public int shotCount = 1;

    public Action onNumberUpdate { get; set; }
    int m_mag;

    public Action<GunFireInfo> modifyFireInfo, onFire;
    public int mag
    {
        get { return m_mag; }
        set
        {
            m_mag = value;
            onNumberUpdate?.Invoke();
        }
    }
    int m_bullets;
    public int bullets
    {
        get { return m_bullets; }
        set
        {
            m_bullets = value;
            onNumberUpdate?.Invoke();
        }
    }
    public string numberText => $"{mag}/{bullets}";

    float counter = 0.0f;
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        counter = fireRate;
    }
    readonly LayerMask whatToHit = LayerMask.GetMask("Map", "Enemy");
    public override void OnWieldUpdate()
    {
        base.OnWieldUpdate();
        if (counter > 0.0f) counter -= Time.deltaTime;
        if(counter <= 0.0f)
        {
            if(auto && InputManager.GetButton() || !auto && InputManager.GetButtonDown())
            {
                if(mag > 0)
                {
                    GunFireInfo fireInfo = new()
                    {
                        damage = damage,
                        fireRate = fireRate,
                        range = range,
                        spread = spread,
                        bulletSpeed = bulletSpeed,
                        pierce = pierce,
                        shotCount = shotCount,
                        startPos = weapon.firePoint.position,
                        source = GunFireSource.GunFire
                    };

                    modifyFireInfo?.Invoke(fireInfo);
                    mag = Mathf.Max(mag - fireInfo.consumedBullet, 0);
                    counter = fireInfo.fireRate;

                    onFire?.Invoke(fireInfo);
                    Fire(fireInfo);
                }
                else
                {

                }
            }
        }
    }
    public void Fire(GunFireInfo fireInfo)
    {
        if(bullet is Projectile)
        {

        }
        else
        {

        }
    }
    void Hit(HitInfo info)
    {

    }
}
public enum GunFireSource
{
    GunFire,
    QuirkTriggered
}
public class GunFireInfo
{
    public float damage, fireRate, range, spread;
    public int consumedBullet = 1;
    public float bulletSpeed;
    public int pierce, shotCount;
    public Vector3 startPos, dir;
    public GunFireSource source;
}
public class GunGiveDamageInfo
{
    public float damage;
    public HitInfo hit;
}