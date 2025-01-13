using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusAliment_Slowed : StatusAliment
{
    public override AlimentType type => AlimentType.Slowed;
    float slowedPercentage = 1.0f;
    float timeLeft = 3.0f;
    public StatusAliment_Slowed()
    {

    }
    public StatusAliment_Slowed(float slowedPercentage, float timeLeft)
    {
        this.slowedPercentage = slowedPercentage;
        this.timeLeft = timeLeft;
    }
    StatusAlimentEffect effect = null;
    public override void OnAlimentAdd()
    {
        base.OnAlimentAdd();
        if(target.TryGetComponent(out ISpeedAdjustable tmp))
        {
            tmp.speed *= slowedPercentage;
            effect = StatusAlimentGlobalData.Instance.slowedEffect.Instantiate();
            effect.transform.SetParent(target.alimentEffectAnchor);
        }
    }
    public override void OnAlimentUpdate()
    {
        base.OnAlimentUpdate();
        if (timeLeft > 0) timeLeft -= Time.deltaTime;
        else RemoveAliment();
    }
    public override void OnAlimentRemove()
    {
        base.OnAlimentRemove();
        if (target.TryGetComponent(out ISpeedAdjustable tmp))
        {
            tmp.speed /= slowedPercentage;
            effect.Release();
        }
    }
}