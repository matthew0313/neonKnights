using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsExplosiveProjectile : PooledPrefab<PhysicsExplosiveProjectile>
{
    Rigidbody rb;
    Action<GameObject> onHit;
    float blastRadius;
    LayerMask whatToHit;
    ExplosionEffect effect;
    protected override void OnCreate()
    {
        base.OnCreate();
        rb = GetComponent<Rigidbody>();
    }
    public void Set(Action<GameObject> onHit, float blastRadius, Vector3 force, LayerMask whatToHit, ExplosionEffect effect)
    {
        this.onHit = onHit;
        this.blastRadius = blastRadius;
        this.whatToHit = whatToHit;
        this.effect = effect;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(force, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (released) return;
        RaycastHit[] hitArr = Physics.SphereCastAll(transform.position, blastRadius, Vector3.up, blastRadius, whatToHit);
        List<RaycastHit> hits = hitArr.ToList();
        hits.Sort((RaycastHit a, RaycastHit b) => { return a.distance.CompareTo(b.distance); });
        List<GameObject> hit = new();
        foreach(var i in hits)
        {
            if (i.transform.TryGetComponent(out Point point))
            {
                if (hit.Contains(point.pointOf)) continue;
                else hit.Add(point.pointOf);
            }
            else
            {
                if (hit.Contains(i.transform.gameObject)) continue;
                else hit.Add(i.transform.gameObject);
            }
            onHit?.Invoke(i.transform.gameObject);
        }
        effect.Instantiate().transform.position = transform.position;
        Release();
    }
}