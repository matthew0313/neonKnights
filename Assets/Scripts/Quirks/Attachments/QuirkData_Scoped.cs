using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Scoped", menuName = "Scriptables/Quirks/Attachments/Scoped", order = 0)]
public class QuirkData_Scoped : QuirkData
{
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is GunItem && target is IScopeAttachable && (target as IScopeAttachable).scopeAttached == false;
    }
    public override Quirk Create()
    {
        return new Quirk_Scoped(this);
    }
}
public class Quirk_Scoped : Quirk
{
    new public readonly QuirkData_Scoped data;
    public Quirk_Scoped(QuirkData_Scoped data) : base(data)
    {
        this.data = data;
    }
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as IScopeAttachable).scopeAttached = true;
        (target as GunItem).spread *= (1.0f - (target as IScopeAttachable).scopeSpreadReduction / 100.0f);
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"-{(target as IScopeAttachable).scopeSpreadReduction}% Spread",
            kr = $"퍼짐 -{(target as IScopeAttachable).scopeSpreadReduction}%"
        };
    }
}