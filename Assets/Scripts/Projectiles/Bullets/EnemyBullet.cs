using System;
using UnityEngine;

public class EnemyBullet : Bullet
{
    Action<GameObject> onHit;
    float speed, range;
    public void Set(Action<GameObject> onHit, float speed, float range)
    {
        this.onHit = onHit;
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
    private void OnTriggerEnter(Collider other)
    {
        GameObject hit = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
        if ((hit.layer | LayerMask.GetMask("Map", "Player")) == 0) return;
        onHit?.Invoke(hit);
        Release();
    }
}