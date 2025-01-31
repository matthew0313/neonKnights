using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
public class Aliment_Slowed : StatusAliment, IAlimentProgressDisplayed
{
    public override AlimentType type => AlimentType.slowed;
    float slowedPercentage = 50.0f;
    float startTime = 3.0f, timeLeft = 3.0f;
    public float progressLeft => timeLeft / startTime;
    public override string alimentDisplayString => $"Slowed {Mathf.Round(slowedPercentage * 10.0f) / 10.0f}%";
    public override Color alimentColor => StatusAlimentGlobalData.Instance.slowedColor;
    public Aliment_Slowed()
    {

    }
    public Aliment_Slowed(float slowedPercentage, float startTime)
    {
        this.slowedPercentage = slowedPercentage;
        this.startTime = startTime;
        timeLeft = startTime;
    }
    StatusAlimentEffect effect = null;
    public override void AddTo(HpComp target)
    {
        if (target.FindAliment(out Aliment_Slowed found)) return;
        else base.AddTo(target);
    }
    public override void OnAlimentAdd()
    {
        base.OnAlimentAdd();
        effect = StatusAlimentGlobalData.Instance.slowedEffect.Instantiate();
        effect.transform.SetParent(target.alimentEffectAnchor, false);
        effect.transform.localPosition = new Vector3(0, 0, 0);
        if (target.TryGetComponent(out ISpeedMultiplierAdjustable tmp))
        {
            tmp.speedMultiplier *= slowedPercentage / 100.0f;
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
        effect.Release();
        if (target.TryGetComponent(out ISpeedMultiplierAdjustable tmp))
        {
            tmp.speedMultiplier /= slowedPercentage / 100.0f;
        }
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats["slowedPercentage"] = slowedPercentage;
        data.floats["startTime"] = startTime;
        data.floats["timeLeft"] = timeLeft;
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        data.floats.TryGetValue("slowedPercentage", out slowedPercentage);
        data.floats.TryGetValue("startTime", out startTime);
        data.floats.TryGetValue("timeLeft", out timeLeft);
    }
}