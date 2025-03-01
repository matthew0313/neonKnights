using System;
using UnityEngine;

public class EnemyBullet : Bullet
{
    [SerializeField] MeshRenderer rend;
    [SerializeField] Material reflectedMaterial;
    float damage, speed, range;
    public void Set(float damage, float speed, float range)
    {
        this.damage = damage;
        this.speed = speed;
        this.range = range;
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
        if (((hit.layer | (reflected ? LayerMask.GetMask("Map", "Enemy") : LayerMask.GetMask("Map", "Player"))) == 0)) return;
        OnHit(other);
        Release();
    }
    public void Reflect()
    {
        if (reflected) return;
        reflected = true;
        rend.material = reflectedMaterial;
        transform.rotation = Quaternion.Euler(-transform.eulerAngles);
    }
    protected virtual void OnHit(Collider other)
    {
        if (other.TryGetComponent(out IDamagable tmp)) tmp.GetDamage(damage);
    }
}