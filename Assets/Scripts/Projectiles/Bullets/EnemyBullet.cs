using System;
using UnityEngine;

public class EnemyBullet : Bullet
{
    [SerializeField] MeshRenderer rend;
    [SerializeField] Material material, reflectedMaterial;
    public float damage, speed, range;
    public void Set(float damage, float speed, float range)
    {
        this.damage = damage;
        this.speed = speed;
        this.range = range;
        reflected = false;
        rend.material = material;
        transform.localScale = Vector3.one;
        counter = 0.0f;
    }
    float counter = 0.0f;
    private void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        counter += Time.deltaTime;
        if (counter * speed > range) Release();
    }
    bool reflected = false;
    private void OnTriggerEnter(Collider other)
    {
        GameObject hit = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
        Debug.Log(hit.name);
        if (hit.layer != LayerMask.NameToLayer("Map"))
        {
            if (!reflected)
            {
                if (hit.layer != LayerMask.NameToLayer("Player")) return;
            }
            else
            {
                if (hit.layer != LayerMask.NameToLayer("Enemy")) return;
            }
        }
        OnHit(hit);
        Release();
    }
    public void Reflect()
    {
        if (reflected) return;
        reflected = true;
        counter = 0.0f;
        rend.material = reflectedMaterial;
    }
    protected virtual void OnHit(GameObject hit)
    {
        if (hit.TryGetComponent(out IDamagable tmp)) tmp.GetDamage(damage);
    }
}