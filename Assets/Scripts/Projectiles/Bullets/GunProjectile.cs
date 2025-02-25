using UnityEngine;
public class GunProjectile : Bullet
{
    /*float speed = 20.0f;
    float range;
    LayerMask whatToHit;

    float counter = 0.0f;
    Action<GunHitInfo> onHit;

    public Action onDespawn;
    public Action<Vector3, Vector3> onTimeout;

    private void OnEnable()
    {
        onDespawn = null;
    }
    public void Set(float speed, float range, LayerMask whatToHit, Action<GunHitInfo> onHit)
    {
        counter = 0.0f;
        this.speed = speed;
        this.range = range;
        this.whatToHit = whatToHit;
        this.onHit = onHit;
    }
    private void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime);
        counter += speed * Time.deltaTime;
        if (counter >= range)
        {
            Release();
            onTimeout?.Invoke(transform.position, transform.forward);
        }
    }
    protected override void OnRelease()
    {
        base.OnRelease();
        onDespawn?.Invoke();
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.layer & whatToHit) == 0) return;
        onHit?.Invoke(new GunHitInfo()
        {
            hitObject = other.gameObject,
            distance = counter,
            hitPos = transform.position,
            hitDir = transform.forward
        });
    }*/
    GunItem gun;
    GunShot shot;
    Player shooter;
    int pierceLeft;

    GunFireInfo fireInfo => shot.origin.fireInfo;
    public void Set(GunItem gun, GunShot shot, Player shooter)
    {
        this.gun = gun;
        this.shot = shot;
        this.shooter = shooter;
        pierceLeft = fireInfo.pierce;
    }
    float counter = 0.0f;
    private void Update()
    {
        transform.Translate(transform.forward * fireInfo.bulletSpeed * Time.deltaTime);
        counter += fireInfo.bulletSpeed * Time.deltaTime;
        if (counter >= fireInfo.range)
        {
            EndShot();
            Release();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.layer & fireInfo.whatToHit) == 0) return;
        GunHitInfo hit = new()
        {
            hitObject = other.gameObject,
            distance = counter,
            hitPos = transform.position,
            hitDir = transform.forward
        };
        if(hit.hitObject.TryGetComponent(out HitPoint point))
        {
            if (shot.hit.Contains(point.pointOf)) return;
            shot.hit.Add(point.pointOf);
        }
        else
        {
            if (shot.hit.Contains(hit.hitObject)) return;
            shot.hit.Add(hit.hitObject);
        }
        gun.onShotHit?.Invoke(shot, hit);
        if(hit.hitObject.TryGetComponent(out IDamagable target))
        {
            GunGiveDamageInfo damageInfo = new()
            {
                damage = fireInfo.damage * (1.0f - fireInfo.bulletDropoffPercentage / 100.0f * Mathf.Min(1.0f, hit.distance / fireInfo.range)),
                hit = hit
            };
            gun.modifyShotDamageGiven?.Invoke(shot, damageInfo);
            gun.onShotDamageGive?.Invoke(shot, target.GetDamage(new()
            {
                amount = damageInfo.damage,
                source = shooter
            }), hit);
            shot.hit.Add(hit.hitObject);
            if (pierceLeft <= 0)
            {
                EndShot();
                Release();
            }
            else pierceLeft--;
        }
        else
        {
            gun.onShotNonDamagableHit?.Invoke(shot, hit);
            EndShot();
            Release();
        }
    }
    void EndShot()
    {
        gun.onShotEnd?.Invoke(shot);
        shot.origin.shots.Remove(shot);
        if (shot.origin.shots.Count == 0) gun.onFireEnd?.Invoke(shot.origin);
    }
    protected override void OnRelease()
    {
        base.OnRelease();
        gun = null;
        shot = null;
    }
}
public struct GunHitInfo
{
    public GameObject hitObject;
    public float distance;
    public Vector3 hitPos, hitDir;

    public static implicit operator bool(GunHitInfo self)
    {
        return self.hitObject != null;
    }
}