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
public abstract class GunItem : WeaponItem, INumberDisplayed, IStatDisplayed, IDamageModifyable
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
    public float damage { get; set; }
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
    public Action<GunFire> onFireEnd;

    public Action<GunShot> onShotStart;
    public Action<GunShot, GunHitInfo> onShotHit;
    public Action<GunShot, DamageReceivedInfo, GunHitInfo> onShotDamageGive;
    public Action<GunShot, GunHitInfo> onShotNonDamagableHit;
    public Action<GunShot> onShotEnd;

    public Action<GunShot, GunGiveDamageInfo> modifyShotDamageGiven;

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
                if ((auto && Input.GetMouseButton(0) || !auto && Input.GetMouseButtonDown(0)) && UIScanner.ScanUI(Input.mousePosition).Count <= 0)
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
                            source = GunFireSource.GunFire,
                            whatToHit = whatToHit
                        };
                        if (SystemInfo.deviceType == DeviceType.Desktop)
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
        reloadCounter = 0.0f;
        reloading = false;
    }
    readonly int fireID = Animator.StringToHash("Fire");
    public void Fire(GunFireInfo fireInfo)
    {
        wielder.animation.TriggerFire();
        //지금 이 한 번의 사격을 의미하는 인스턴스 생성
        GunFire fire = new()
        {
            fireInfo = fireInfo
        };
        onFireStart?.Invoke(fire);
        if (fireInfo.bullet is GunProjectile)
        {
            GunShot[] shots = new GunShot[fireInfo.shotCount];
            for (int i = 0; i < fireInfo.shotCount; i++)
            {
                GunShot shot = new()
                {
                    origin = fire
                };
                fire.shots.Add(shot);
                shots[i] = shot;
            }
            for(int i = 0; i < shots.Length; i++)
            {
                GunProjectile bul = fireInfo.bullet.Instantiate() as GunProjectile;
                bul.transform.position = fireInfo.startPos;
                bul.transform.forward = Quaternion.Euler(UnityEngine.Random.Range(-fireInfo.spread, fireInfo.spread), UnityEngine.Random.Range(-fireInfo.spread, fireInfo.spread), UnityEngine.Random.Range(-fireInfo.spread, fireInfo.spread)) * fireInfo.dir;
                bul.Set(this, shots[i]);
            }
        }
        else if(fireInfo.bullet is HitscanEffect)
        {
            GunShot[] shots = new GunShot[fireInfo.shotCount];
            for (int i = 0; i < fireInfo.shotCount; i++)
            {
                GunShot shot = new()
                {
                    origin = fire
                };
                fire.shots.Add(shot);
                shots[i] = shot;
            }
            for (int i = 0; i < shots.Length; i++)
            {
                GunShot shot = shots[i];
                Vector3 dir = Quaternion.Euler(UnityEngine.Random.Range(-fireInfo.spread, fireInfo.spread), UnityEngine.Random.Range(-fireInfo.spread, fireInfo.spread), UnityEngine.Random.Range(-fireInfo.spread, fireInfo.spread)) * fireInfo.dir;
                List<RaycastHit> hits = Physics.RaycastAll(fireInfo.startPos, dir, fireInfo.range, whatToHit).ToList();
                float dist = fireInfo.range;
                hits.Sort((a, b) => a.distance.CompareTo(b.distance));
                for (int k = 0; k < hits.Count; k++)
                {
                    GunHitInfo hit = new()
                    {
                        hitObject = hits[k].transform.gameObject,
                        distance = hits[k].distance,
                        hitPos = hits[k].point,
                        hitDir = dir
                    };
                    if (shot.hit.Contains(hit.hitObject)) continue;
                    onShotHit?.Invoke(shot, hit);
                    if (hit.hitObject.TryGetComponent(out IDamagable target))
                    {
                        GunGiveDamageInfo damageInfo = new()
                        {
                            damage = fireInfo.damage * (1.0f - fireInfo.bulletDropoffPercentage / 100.0f * Mathf.Min(1.0f, hit.distance / fireInfo.range)),
                            hit = hit
                        };
                        modifyShotDamageGiven?.Invoke(shot, damageInfo);
                        onShotDamageGive?.Invoke(shot, target.GetDamage(damageInfo.damage), hit);
                        shot.hit.Add(hit.hitObject);
                        if (pierce <= 0)
                        {
                            dist = hit.distance;
                            break;
                        }
                        else pierce--;
                    }
                    else
                    {
                        dist = hit.distance;
                        onShotNonDamagableHit?.Invoke(shot, hit);
                        break;
                    }
                }
                onShotEnd?.Invoke(shot);

                HitscanEffect bul = fireInfo.bullet.Instantiate() as HitscanEffect;
                bul.transform.position = fireInfo.startPos;
                bul.transform.forward = dir;
                bul.Set(dist);
            }
            onFireEnd?.Invoke(fire);
        }
    }
    public string numberText => $"{mag}/{reserveLeft}";
    public IEnumerable<LangText> GetStats()
    {
        yield return new()
        {
            en = $"Damage: {damage}" + (shotCount > 1 ? $"x{shotCount}" : ""),
            kr = $"대미지: {damage}" + (shotCount > 1 ? $"x{shotCount}" : "")
        };
        yield return new()
        {
            en = $"FireRate: {fireRate}s",
            kr = $"발사 주기: {fireRate}초"
        };
        yield return new()
        {
            en = $"Range: {range}",
            kr = $"범위: {range}"
        };
        yield return new()
        {
            en = $"Spread: {spread}",
            kr = $"탄 퍼짐: {spread}"
        };
        yield return new()
        {
            en = $"Reload Time: {reloadTime}s",
            kr = $"재장전 시간: {reloadTime}초"
        };
        yield return new()
        {
            en = $"Mag: {magSize}",
            kr = $"탄창: {magSize}"
        };
        yield return new()
        {
            en = $"Reserve: {reserve}",
            kr = $"탄 저장량: {reserve}"
        };
        yield return new()
        {
            en = $"Dropoff: {bulletDropoffPercentage}%",
            kr = $"탄 투하: {bulletDropoffPercentage}%"
        };
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
    public LayerMask whatToHit;

    public void PrintDebug()
    {
        Debug.Log($"damage: {damage}\nfireRate: {fireRate}\nrange: {range}\nspread: {spread}\nconsumedBullet: {consumedBullet}\nbullet: {bullet}\nbulletSpeed: {bulletSpeed}\npierce: {pierce}\nshotCount: {shotCount}\nbulletDropoffPercentage: {bulletDropoffPercentage}\nstartPos: {startPos}\ndir: {dir}\nsource: {source}");
    }
}
public class GunGiveDamageInfo
{
    public DamageInfo damage;
    public GunHitInfo hit;
}
//한 번의 사격
public class GunFire
{
    public GunFireInfo fireInfo;
    public List<GunShot> shots = new();
}
//한 번의 사격에서 생긴 한 번의 발사
//샷건은 10개, 권총은 1개 등등
public class GunShot
{
    public GunFire origin;
    public List<GameObject> hit = new();
}
public struct GunShotEndInfo
{
    public bool endedViaObjectHit;
    public Vector3 pos, dir;
}