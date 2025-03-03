using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy_HpComp : HpComp
{
    [Header("Enemy_HpComp")]
    [SerializeField] float hitDetection = 50.0f;
    [SerializeField] LootGen loot;

    [SerializeField] UnityEvent onEnemyDeath;

    protected Enemy origin;
    protected override void Awake()
    {
        base.Awake();
        origin = GetComponent<Enemy>();
    }
    internal bool looted = false;
    const float itemDropForce = 3.0f;
    protected override void OnDeath()
    {
        base.OnDeath();
        if (looted || loot == null) return;
        else
        {
            looted = true;
            foreach(var i in loot.GetLoot())
            {
                DroppedItem tmp = DroppedItem.CreateDroppedItem(i.item, i.count);
                tmp.transform.position = transform.position;

                float ang = UnityEngine.Random.Range(0.0f, 360.0f);
                tmp.rb.AddForce(new Vector3(Mathf.Sin(ang) * itemDropForce, itemDropForce, Mathf.Cos(ang) * itemDropForce), ForceMode.VelocityChange);
            }
        }
        onEnemyDeath?.Invoke();
        gameObject.SetActive(false);
    }
    protected override void OnLoad()
    {
        base.OnLoad();
        if (dead)
        {
            gameObject.SetActive(false);
            Debug.Log("Dead");
        }
    }
    public override DamageReceivedInfo GetDamage(DamageInfo damage)
    {
        if(damage.source is Player) origin.AddDetection(hitDetection);
        return base.GetDamage(damage);
    }
}