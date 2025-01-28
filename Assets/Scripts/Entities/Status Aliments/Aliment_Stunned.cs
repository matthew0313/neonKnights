using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
public class Aliment_Stunned : StatusAliment, IAlimentProgressDisplayed
{
    public override AlimentType type => AlimentType.stunned;
    float startTime = 3.0f, timeLeft = 3.0f;
    public float progressLeft => timeLeft / startTime;
    public override string alimentDisplayString => "Stunned";
    public override Color alimentColor => StatusAlimentGlobalData.Instance.stunnedColor;
    public Aliment_Stunned()
    {

    }
    public Aliment_Stunned(float startTime)
    {
        this.startTime = startTime;
        timeLeft = startTime;
    }
    StatusAlimentEffect effect = null;
    public override void AddTo(HpComp target)
    {
        if(target.FindAliment(out Aliment_Stunned found))
        {
            if(timeLeft > found.timeLeft)
            {
                found.startTime = startTime;
                found.timeLeft = timeLeft;
                TextIndicator.Indicate(target.transform.position, alimentDisplayString, alimentColor);
            }
        }
        else base.AddTo(target);
    }
    public override void OnAlimentAdd()
    {
        base.OnAlimentAdd();
        effect = StatusAlimentGlobalData.Instance.stunnedEffect.Instantiate();
        effect.transform.SetParent(target.alimentEffectAnchor, false);
        if (target.TryGetComponent(out IStunable tmp)) tmp.stunned = true;
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
        if (target.TryGetComponent(out IStunable tmp)) tmp.stunned = false;
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats["startTime"] = startTime;
        data.floats["timeLeft"] = timeLeft;
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        data.floats.TryGetValue("startTime", out startTime);
        data.floats.TryGetValue("timeLeft", out timeLeft);
    }
}