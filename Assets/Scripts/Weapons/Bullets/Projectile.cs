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
    
    public Action<Projectile, HitInfo> onHit;
    public void Set(float speed, float range, LayerMask whatToHit)
    {
        counter = 0.0f;
        this.speed = speed;
        this.range = range;
        this.whatToHit = whatToHit;
    }
    float counter = 0.0f;
    private void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime);
        counter += speed * Time.deltaTime;
        if (counter >= range)
        {
            Release();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.layer & whatToHit) == 0) return;
        onHit?.Invoke(this, new HitInfo()
        {
            hitObject = other.gameObject,
            distance = counter,
            hitPos = transform.position,
            hitDir = transform.forward
        });
    }
}
public struct HitInfo
{
    public GameObject hitObject;
    public float distance;
    public Vector3 hitPos, hitDir;

    public static implicit operator bool(HitInfo self)
    {
        return self.hitObject != null;
    }
}