using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Swift", menuName = "Scriptables/Quirks/Guns/Swift", order = 1)]
public class QuirkData_Swift : QuirkData
{
    [Header("Speed Boost")]
    [SerializeField] float m_minSpeedIncrease = 40.0f;
    [SerializeField] float m_maxSpeedIncrease = 40.0f;
    
    public float minSpeedIncrease => m_minSpeedIncrease;
    public float maxSpeedIncrease => m_maxSpeedIncrease;
    
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is GunItem;
    }
    public override Quirk Create()
    {
        return new Quirk_Swift(this);
    }
}

public class Quirk_Swift : Quirk
{
    new public readonly QuirkData_Swift data;
    private float speedIncrease;
    private GunItem gunItem;
    
    public Quirk_Swift(QuirkData_Swift data) : base(data)
    {
        this.data = data;
        this.speedIncrease = data.minSpeedIncrease; 
    }
    
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        if (target is GunItem gun)
        {
            gunItem = gun;
            if (gunItem.wielder != null && gunItem.wielder.movement != null)
            {
                gunItem.wielder.movement.moveSpeed *= (1.0f + speedIncrease / 100.0f);
            }
        }
    }
    
    public new void OnQuirkRemove(IQuirkable target)
    {
        if (gunItem != null && gunItem.wielder != null && gunItem.wielder.movement != null)
        {
            gunItem.wielder.movement.moveSpeed /= (1.0f + speedIncrease / 100.0f);
        }
        base.OnQuirkRemove(target);
    }
    
    public override void SetRandom()
    {
        speedIncrease = UnityEngine.Random.Range(data.minSpeedIncrease, data.maxSpeedIncrease);
    }
    
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"Increases movement speed by {speedIncrease}% while holding a gun.",
            kr = $"총을 들고 있을 때 이동 속도가 {speedIncrease}% 증가합니다."
        };
    }
    
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats["speedIncrease"] = speedIncrease;
    }
    
    public override void Load(DataUnit data)
    {
        base.Load(data);
        data.floats.TryGetValue("speedIncrease", out speedIncrease);
    }
    
    public override Quirk Copy()
    {
        Quirk_Swift tmp = base.Copy() as Quirk_Swift;
        tmp.speedIncrease = speedIncrease;
        return tmp;
    }
}
