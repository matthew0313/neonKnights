using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.VisualScripting.Member;

public abstract class GunItemData : WeaponItemData
{
    [Header("Default Stats")]
    [SerializeField] bool m_auto;
    [SerializeField] float m_baseDamage;
    [SerializeField] float m_baseFireRate, m_baseRange, m_baseSpread;
    [SerializeField] int m_baseMagSize, m_baseReloadTime, m_baseReserve, m_baseShotCount = 1;
    [SerializeField] Bullet m_defaultBullet;
    [SerializeField] float m_baseBulletSpeed;
    [SerializeField] int m_defaultPierce = 0;
    [SerializeField] float m_defaultBulletDropoffPercentage = 50.0f;
    public bool auto => m_auto;
    public float baseDamage => m_baseDamage;
    public float baseFireRate => m_baseFireRate;
    public float baseRange => m_baseRange;
    public float baseSpread => m_baseSpread;
    public int baseMagSize => m_baseMagSize;
    public float baseReloadTime => m_baseReloadTime;
    public int baseReserve => m_baseReserve;
    public int baseShotCount => m_baseShotCount;
    public Bullet defaultBullet => m_defaultBullet;
    public float baseBulletSpeed => m_baseBulletSpeed;
    public int defaultPierce => m_defaultPierce;
    public float defaultBulletDropoffPercentage => m_defaultBulletDropoffPercentage;

}
public abstract class GunItem : WeaponItem, INumberDisplayed, IStatDisplayed
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
        mag = magSize;
        reloadTime = data.baseReloadTime;
        reserve = data.baseReserve;
        reserveLeft = reserve;
        bullet = data.defaultBullet;
        bulletSpeed = data.baseBulletSpeed;
        pierce = data.defaultPierce;
        shotCount = data.baseShotCount;
        bulletDropoffPercentage = data.defaultBulletDropoffPercentage;
    }
    //stats
    public bool auto;
    public float damage;
    public float fireRate;
    public float range;
    public float spread;
    public int magSize;
    public float reloadTime;
    public int reserve;
    public Bullet bullet;
    public float bulletSpeed;
    public int pierce = 0;
    public int shotCount = 1;
    public float bulletDropoffPercentage;

    public Action onNumberUpdate { get; set; }
    int m_mag;

    public Action<GunFireInfo> modifyFireInfo;
    public Action<GunFire> onFireStart;
    public Action<GunFire> onFireObjectHit;
    public Action<GunFire> onFireEnd;
    public Action<GunShotEndInfo> onShotEnd;
    public Action<GunGiveDamageInfo> modifyDamageGiven;
    public Action<DamageReceivedInfo> onDamageGive;
    public Action onReload;
    public int mag
    {
        get { return m_mag; }
        set
        {
            m_mag = value;
            onNumberUpdate?.Invoke();
        }
    }
    int m_reserveLeft;
    public int reserveLeft
    {
        get { return m_reserveLeft; }
        set
        {
            m_reserveLeft = value;
            onNumberUpdate?.Invoke();
        }
    }

    float counter = 0.0f;
    bool reloading = false;
    float reloadCounter = 0.0f;
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
        if (reloading)
        {
            reloadCounter += Time.deltaTime;
            if(reloadCounter >= reloadTime)
            {
                int amount = Mathf.Min(magSize - mag, reserveLeft);
                mag += amount;
                reserveLeft -= amount;
                reloading = false;
                onReload?.Invoke();
            }
        }
        else
        {
            if (InputManager.GetReloadInput() && mag < magSize && reserveLeft > 0)
            {
                reloadCounter = 0.0f;
                reloading = true;
            }
            else if(counter <= 0.0f)
            {
                if ((auto && InputManager.GetButton() || !auto && InputManager.GetButtonDown()) && UIScanner.ScanUI(InputManager.GetMouseInput()).Count <= 0)
                {
                    if (mag > 0)
                    {
                        GunFireInfo fireInfo = new()
                        {
                            damage = damage,
                            fireRate = fireRate,
                            range = range,
                            spread = spread,
                            bullet = bullet,
                            bulletSpeed = bulletSpeed,
                            pierce = pierce,
                            shotCount = shotCount,
                            bulletDropoffPercentage = bulletDropoffPercentage,
                            startPos = weapon.firePoint.position,
                            source = GunFireSource.GunFire
                        };
                        if (SystemInfo.deviceType == DeviceType.Desktop)
                        {
                            Ray ray = Camera.main.ScreenPointToRay(InputManager.GetMouseInput());
                            float height = wielder.transform.position.y - ray.origin.y;
                            Vector3 end = ray.origin + ray.direction * (height / ray.direction.y);
                            fireInfo.dir = (end - weapon.firePoint.position).normalized;
                        }

                        modifyFireInfo?.Invoke(fireInfo);
                        mag = Mathf.Max(mag - fireInfo.consumedBullet, 0);
                        counter = fireInfo.fireRate;

                        Fire(fireInfo);
                    }
                    else
                    {

                    }
                }
            }
        }
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        reloading = false;
    }
    public void Fire(GunFireInfo fireInfo)
    {
        GunFire fire = new()
        {
            fireInfo = fireInfo
        };
        onFireStart?.Invoke(fire);
        if (fireInfo.bullet is Projectile)
        {
            int despawned = 0;
            for(int i = 0; i < fireInfo.shotCount; i++)
            {
                int pierce = fireInfo.pierce;
                Projectile bul = fireInfo.bullet.Instantiate() as Projectile;
                bul.transform.position = fireInfo.startPos;
                bul.transform.forward = Quaternion.Euler(UnityEngine.Random.Range(-fireInfo.spread, fireInfo.spread), UnityEngine.Random.Range(-fireInfo.spread, fireInfo.spread), UnityEngine.Random.Range(-fireInfo.spread, fireInfo.spread)) * fireInfo.dir;
                bul.Set(fireInfo.bulletSpeed, fireInfo.range, whatToHit, (HitInfo hit) =>
                {
                    onFireObjectHit?.Invoke(fire);
                    if (hit.hitObject.TryGetComponent(out IDamagable target))
                    {
                        GunGiveDamageInfo damageInfo = new()
                        {
                            damage = fireInfo.damage * (1.0f - fireInfo.bulletDropoffPercentage / 100.0f * Mathf.Min(1.0f, hit.distance / fireInfo.range)),
                            hit = hit
                        };
                        modifyDamageGiven?.Invoke(damageInfo);
                        onDamageGive?.Invoke(target.GetDamage(damageInfo.damage));
                        if (pierce <= 0)
                        {
                            bul.Release();
                            onShotEnd?.Invoke(new()
                            {
                                endedViaObjectHit = true,
                                pos = hit.hitPos,
                                dir = hit.hitDir
                            });
                        }
                        else pierce--;
                    }
                    else
                    {
                        bul.Release();
                        onShotEnd?.Invoke(new()
                        {
                            endedViaObjectHit = true,
                            pos = hit.hitPos,
                            dir = hit.hitDir
                        });
                    }
                });
                bul.onDespawn = () =>
                {
                    despawned++;
                    if (despawned >= fireInfo.shotCount)
                    {
                        onFireEnd?.Invoke(fire);
                    }
                };
                bul.onTimeout = (pos, dir) =>
                {
                    onShotEnd?.Invoke(new()
                    {
                        endedViaObjectHit = false,
                        pos = pos,
                        dir = dir
                    });
                };
            }
        }
        else if(fireInfo.bullet is HitscanEffect)
        {
            for(int i = 0; i < fireInfo.shotCount; i++)
            {
                Vector3 dir = Quaternion.Euler(UnityEngine.Random.Range(-fireInfo.spread, fireInfo.spread), UnityEngine.Random.Range(-fireInfo.spread, fireInfo.spread), UnityEngine.Random.Range(-fireInfo.spread, fireInfo.spread)) * fireInfo.dir;
                List<RaycastHit> hits = Physics.RaycastAll(fireInfo.startPos, dir, fireInfo.range, whatToHit).ToList();
                float dist = fireInfo.range;
                if(hits.Count > 0)
                {
                    hits.Sort((a, b) => a.distance.CompareTo(b.distance));
                    for (int k = 0; k < hits.Count; k++)
                    {
                        HitInfo hit = new()
                        {
                            hitObject = hits[k].transform.gameObject,
                            distance = hits[k].distance,
                            hitPos = hits[k].point,
                            hitDir = dir
                        };
                        onFireObjectHit?.Invoke(fire);
                        if (hit.hitObject.TryGetComponent(out IDamagable target))
                        {
                            GunGiveDamageInfo damageInfo = new()
                            {
                                damage = fireInfo.damage * (1.0f - fireInfo.bulletDropoffPercentage / 100.0f * Mathf.Min(1.0f, hit.distance / fireInfo.range)),
                                hit = hit
                            };
                            modifyDamageGiven?.Invoke(damageInfo);
                            onDamageGive?.Invoke(target.GetDamage(damageInfo.damage));
                            if (pierce <= 0)
                            {
                                dist = hit.distance;
                                onShotEnd?.Invoke(new()
                                {
                                    endedViaObjectHit = true,
                                    pos = hit.hitPos,
                                    dir = hit.hitDir
                                });
                                break;
                            }
                            else pierce--;
                        }
                        else
                        {
                            dist = hit.distance;
                            onShotEnd?.Invoke(new()
                            {
                                endedViaObjectHit = true,
                                pos = hit.hitPos,
                                dir = hit.hitDir
                            });
                        }
                    }
                }
                else
                {
                    onShotEnd?.Invoke(new()
                    {
                        endedViaObjectHit = false,
                        pos = fireInfo.startPos + fireInfo.dir * fireInfo.range,
                        dir = dir
                    });
                }
                HitscanEffect bul = fireInfo.bullet.Instantiate() as HitscanEffect;
                bul.transform.position = fireInfo.startPos;
                bul.transform.forward = dir;
                bul.Set(dist);
            }
            onFireEnd?.Invoke(fire);
        }
    }
    public string numberText => $"{mag}/{reserveLeft}";
    public IEnumerable<string> GetStats()
    {
        yield return $"Damage: {damage}" + (shotCount > 1 ? $"x{shotCount}" : "");
        yield return $"FireRate: {fireRate}";
        yield return $"Range: {range}";
        yield return $"Spread: {spread}";
        yield return $"Reload Time: {reloadTime}s";
        yield return $"Mag: {magSize}";
        yield return $"Reserve: {reserve}";
        yield return $"Dropoff: {bulletDropoffPercentage}%";
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
    public Bullet bullet;
    public float bulletSpeed;
    public int pierce, shotCount;
    public float bulletDropoffPercentage;
    public Vector3 startPos, dir;
    public GunFireSource source;

    public void PrintDebug()
    {
        Debug.Log($"damage: {damage}\nfireRate: {fireRate}\nrange: {range}\nspread: {spread}\nconsumedBullet: {consumedBullet}\nbullet: {bullet}\nbulletSpeed: {bulletSpeed}\npierce: {pierce}\nshotCount: {shotCount}\nbulletDropoffPercentage: {bulletDropoffPercentage}\nstartPos: {startPos}\ndir: {dir}\nsource: {source}");
    }
}
public class GunGiveDamageInfo
{
    public float damage;
    public HitInfo hit;
}
public class GunFire
{
    public GunFireInfo fireInfo;
}
public struct GunShotEndInfo
{
    public bool endedViaObjectHit;
    public Vector3 pos, dir;
}