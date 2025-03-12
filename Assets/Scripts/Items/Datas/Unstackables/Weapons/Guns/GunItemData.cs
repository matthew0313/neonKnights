using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class GunItemData : WeaponItemData
{
    [Header("Audio")]
    [SerializeField] Sound m_fireSound;
    [SerializeField] Sound m_reloadSound;
    public Sound fireSound => m_fireSound;
    public Sound reloadSound => m_reloadSound;

    [Header("Effects")]
    [SerializeField] HitEffect m_defaultHitEffect;
    public HitEffect defaultHitEffect => m_defaultHitEffect;

    [Header("Default Stats")]
    [SerializeField] bool m_auto;
    [SerializeField] float m_baseDamage;
    [SerializeField] float m_baseFireRate, m_baseRange, m_baseSpread;
    [SerializeField] int m_baseMagSize;
    [SerializeField] float m_baseReloadTime;
    [SerializeField] int m_baseReserve, m_baseShotCount = 1;
    [SerializeField] Bullet m_defaultBullet;
    [SerializeField] float m_baseBulletSpeed;
    [SerializeField] int m_defaultPierce = 0;
    [SerializeField] float m_defaultBulletDropoffPercentage = 50.0f;
    [SerializeField] float m_baseRecoil = 0;
    [SerializeField] float m_baseWeight = 0;
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
    public float baseRecoil => m_baseRecoil;
    public float baseWeight => m_baseWeight;

}
public abstract class GunItem : WeaponItem, INumberDisplayed, IStatDisplayed, IDamageModifyable, IReloadDisplayed
{
    new public readonly GunItemData data;
    new public Gun weapon => base.weapon as Gun;
    public GunItem(GunItemData data) : base(data)
    {
        this.data = data;
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
        recoil = data.baseRecoil;
        weight = data.baseWeight;
        hitEffect = data.defaultHitEffect;
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
    public float bulletDropoffPercentage = 50.0f;
    public float recoil = 0;
    public float weight = 0;
    public HitEffect hitEffect;

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
    public bool reloading { get; private set; } = false;
    public float reloadProgress => reloadCounter / reloadTime;
    float reloadCounter = 0.0f;

    bool aiming = false;
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        counter = fireRate;
        wielder.movement.moveSpeed -= weight;
    }
    readonly LayerMask whatToHit = LayerMask.GetMask("Map", "Enemy");
    PlayingAudio reloadAudio;
    public override void OnWieldUpdate()
    {
        base.OnWieldUpdate();
        if (counter > 0.0f)
        {
            counter -= Time.deltaTime;
        }
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
            SetAimState(Input.GetMouseButton(1), wielder);

            if (InputManager.GetReloadInput()) StartReload();
            else if (counter <= 0.0f)
            {
                if ((auto && Input.GetMouseButton(0) || !auto && Input.GetMouseButtonDown(0)) && UIScanner.ScanUI().Count <= 0)
                {
                    if (mag > 0)
                    {
                        GunFireInfo fireInfo = new()
                        {
                            damage = damage * wielder.statistics.damageMultiplier,
                            fireRate = fireRate,
                            range = range,
                            spread = spread * (aiming ? aimSpreadReduction : 1.0f),
                            bullet = bullet,
                            bulletSpeed = bulletSpeed,
                            pierce = pierce,
                            shotCount = shotCount,
                            bulletDropoffPercentage = bulletDropoffPercentage,
                            recoil = recoil,
                            startPos = weapon.firePoint.position,
                            source = GunFireSource.GunFire,
                            whatToHit = whatToHit,
                            hitEffect = hitEffect
                        };

                        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
                        if (Physics.Raycast(ray, out RaycastHit hit, 100.0f, LayerMask.GetMask("Map", "Enemy")))
                        {
                            fireInfo.dir = hit.point - fireInfo.startPos;
                        }
                        else
                        {
                            fireInfo.dir = ray.origin + ray.direction * 100.0f - fireInfo.startPos;
                        }
                        fireInfo.dir.Normalize();

                        modifyFireInfo?.Invoke(fireInfo);
                        mag = Mathf.Max(mag - fireInfo.consumedBullet, 0);
                        counter = fireInfo.fireRate;

                        AudioManager.Instance.PlaySound(data.fireSound);
                        Fire(fireInfo);
                        wielder.camera.ApplyRecoil(fireInfo.recoil);

                        if (mag <= 0 && Settings.autoReload) StartReload();
                    }
                    else if (Settings.autoReload) StartReload();
                }
            }
        }
    }
    void StartReload()
    {
        if (reloading || reserveLeft <= 0 || mag >= magSize) return;
        reloading = true;

        SetAimState(false, wielder);

        reloadCounter = 0.0f;
        wielder.animation.TriggerReload();
        reloadAudio = AudioManager.Instance.PlaySound(data.reloadSound);
    }
    const float aimSlowdown = 0.8f, aimSpreadReduction = 0.75f;
    void SetAimState(bool aiming, Player wielder)
    {
        if (this.aiming == aiming) return;
        if (this.aiming)
        {
            wielder.statistics.speedMultiplier /= aimSlowdown;
        }
        else
        {
            wielder.statistics.speedMultiplier *= aimSlowdown;
        }
        this.aiming = aiming;
        wielder.camera.zooming = aiming;
        wielder.animation.SetAiming(aiming);
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        if (reloading)
        {
            reloadCounter = 0.0f;
            reloading = false;
            reloadAudio?.Stop();
        }
        SetAimState(false, wielder);

        wielder.movement.moveSpeed += weight;
    }
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
                bul.Set(this, shots[i], wielder);
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
                int pierce = fireInfo.pierce;
                for (int k = 0; k < hits.Count; k++)
                {
                    GunHitInfo hit = new()
                    {
                        hitObject = hits[k].transform.gameObject,
                        distance = hits[k].distance,
                        hitPos = hits[k].point,
                        hitDir = dir
                    };
                    if (hit.hitObject.TryGetComponent(out Point point))
                    {
                        if(shot.hit.Contains(point.pointOf)) continue;
                        shot.hit.Add(point.pointOf);
                    }
                    else
                    {
                        if (shot.hit.Contains(hit.hitObject)) continue;
                        shot.hit.Add(hit.hitObject);
                    }
                    onShotHit?.Invoke(shot, hit);
                    if (hit.hitObject.TryGetComponent(out IDamagable target))
                    {
                        GunGiveDamageInfo damageInfo = new()
                        {
                            damage = fireInfo.damage * (1.0f - fireInfo.bulletDropoffPercentage / 100.0f * Mathf.Min(1.0f, hit.distance / fireInfo.range)),
                            hit = hit
                        };
                        modifyShotDamageGiven?.Invoke(shot, damageInfo);
                        if(fireInfo.hitEffect != null)
                        {
                            HitEffect tmp = fireInfo.hitEffect.Instantiate();
                            tmp.transform.SetParent(hit.hitObject.transform);
                            tmp.transform.position = hit.hitPos;
                        }
                        DamageReceivedInfo damageReceivedInfo = target.GetDamage(damageInfo.damage);
                        onShotDamageGive?.Invoke(shot, damageReceivedInfo, hit);
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
    public Action<GunAmmoRefillInfo> modifyAmmoRefilled, onAmmoRefill;
    internal object characterController;

    public void RefillAmmo(GunAmmoRefillInfo info)
    {
        modifyAmmoRefilled?.Invoke(info);

        reserveLeft = Mathf.Min(reserveLeft + Mathf.CeilToInt(reserve * info.refillPercentage / 100.0f), reserve);
        onAmmoRefill?.Invoke(info);
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
        yield return new()
        {
            en = $"Recoil: {recoil}",
            kr = $"반동: {recoil}"
        };
        yield return new()
        {
            en = $"Weight: {weight}",
            kr = $"무게: {weight}"
        };
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.integers["mag"] = mag;
        data.integers["reserveLeft"] = reserveLeft;
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        if (data.integers.TryGetValue("mag", out int tmp)) mag = tmp;
        if (data.integers.TryGetValue("reserveLeft", out int tmp2)) reserveLeft = tmp2;
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
    public float recoil;
    public Vector3 startPos, dir;
    public GunFireSource source;
    public LayerMask whatToHit;
    public HitEffect hitEffect;

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
public class GunAmmoRefillInfo
{
    public float refillPercentage;
}
public struct GunShotEndInfo
{
    public bool endedViaObjectHit;
    public Vector3 pos, dir;
}