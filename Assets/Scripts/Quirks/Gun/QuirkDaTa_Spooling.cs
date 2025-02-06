using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Spooling", menuName = "Scriptables/Quirks/Guns/Spooling", order = 1)]
public class QuirkData_Spooling : QuirkData
{
    [SerializeField] float reductionPerShot = 0.1f;
    [SerializeField] float minMultiplier = 0.5f;
    [SerializeField] float spoolResetTime = 2.0f;

    public float ReductionPerShot => reductionPerShot;
    public float MinMultiplier => minMultiplier;
    public float SpoolResetTime => spoolResetTime;

    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is GunItem;
    }

    public override Quirk Create()
    {
        return new Quirk_Spooling(this);
    }
}

public class Quirk_Spooling : Quirk
{
    new public readonly QuirkData_Spooling data;
    private GunItem gun;
    private float currentMultiplier;
    private float lastShotTime;
    private float originalFireRate;

    public Quirk_Spooling(QuirkData_Spooling data) : base(data)
    {
        this.data = data;
    }

    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        gun = target as GunItem;
        if (gun != null)
        {
            originalFireRate = gun.fireRate;
            currentMultiplier = 1.0f;
            lastShotTime = 0f;
            gun.onFireStart += OnFireStart;
        }
    }

    private void OnFireStart(GunFire fire)
    {
        if (Time.time - lastShotTime > data.SpoolResetTime)
        {
            currentMultiplier = 1.0f;
        }
        else
        {
            currentMultiplier = Mathf.Max(currentMultiplier - data.ReductionPerShot, data.MinMultiplier);
        }
        lastShotTime = Time.time;
        gun.fireRate = originalFireRate * currentMultiplier;
    }

    public new void OnQuirkRemove(IQuirkable target)
    {
        if (gun != null)
        {
            gun.onFireStart -= OnFireStart;
            gun.fireRate = originalFireRate;
        }
        base.OnQuirkRemove(target);
    }

    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"Spooling: Continuous shots reduce fire rate to {data.MinMultiplier * 100f}% of base rate.",
            kr = $"스풀링: 연속 사격 시 발사 주기가 기본의 {data.MinMultiplier * 100f}%까지 감소합니다."
        };
    }

    public override void Save(DataUnit dataUnit)
    {
        base.Save(dataUnit);
        dataUnit.floats["currentMultiplier"] = currentMultiplier;
        dataUnit.floats["lastShotTime"] = lastShotTime;
        dataUnit.floats["originalFireRate"] = originalFireRate;
    }

    public override void Load(DataUnit dataUnit)
    {
        base.Load(dataUnit);
        dataUnit.floats.TryGetValue("currentMultiplier", out currentMultiplier);
        dataUnit.floats.TryGetValue("lastShotTime", out lastShotTime);
        dataUnit.floats.TryGetValue("originalFireRate", out originalFireRate);
    }

    public override Quirk Copy()
    {
        Quirk_Spooling tmp = base.Copy() as Quirk_Spooling;
        tmp.currentMultiplier = currentMultiplier;
        tmp.lastShotTime = lastShotTime;
        tmp.originalFireRate = originalFireRate;
        return tmp;
    }
}
