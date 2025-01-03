using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class Projectile : Bullet
{
    float speed = 20.0f;
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