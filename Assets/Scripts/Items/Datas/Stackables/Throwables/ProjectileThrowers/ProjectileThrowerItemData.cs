using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
public abstract class ProjectileThrowerItemData : ThrowableItemData
{
    [Header("ProjectileThrower")]
    [SerializeField] PhysicsExplosiveProjectile m_projectile;
    [SerializeField] ExplosionEffect m_effect;
    [SerializeField] float m_throwForce, m_blastRadius;
    [SerializeField] LayerMask m_whatToHit;
    public PhysicsExplosiveProjectile projectile => m_projectile;
    public ExplosionEffect effect => m_effect;
    public float throwForce => m_throwForce;
    public float blastRadius => m_blastRadius;
    public LayerMask whatToHit => m_whatToHit;
}
public abstract class ProjectileThrowerItem : ThrowableItem
{
    new public readonly ProjectileThrowerItemData data;
    public ProjectileThrowerItem(ProjectileThrowerItemData data) : base(data)
    {
        this.data = data;
    }
    const float scanDetectRange = 15.0f;
    protected override void OnThrow()
    {
        PhysicsExplosiveProjectile projectile = data.projectile.Instantiate();
        projectile.transform.position = throwable.transform.position;

        Vector3 dir;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
        {
            dir = hit.point - throwable.transform.position;
        }
        else
        {
            dir = ray.origin + ray.direction * 100.0f - throwable.transform.position;
        }
        dir.Normalize();

        Debug.DrawLine(throwable.transform.position, throwable.transform.position + dir * data.throwForce, Color.yellow, 30.0f);
        projectile.Set(OnHit, data.blastRadius, dir * data.throwForce, data.whatToHit, data.effect);
    }
    protected abstract void OnHit(GameObject hit);
    public override IEnumerable<LangText> GetStats()
    {
        yield return new()
        {
            en = $"Throw Force: {data.throwForce}",
            kr = $"던지는 힘: {data.throwForce}"
        };
        yield return new()
        {
            en = $"Effect Radius: {data.blastRadius}",
            kr = $"효과 범위: {data.blastRadius}"
        };
        foreach (var i in base.GetStats()) yield return i;
    }
}