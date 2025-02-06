using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Cursed", menuName = "Scriptables/Quirks/Special/Cursed", order = 0)]
public class QuirkData_Cursed : QuirkData
{
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is ICurseable && (target as ICurseable).cursed == false;
    }
    public override Quirk Create()
    {
        return new Quirk_Cursed(this);
    }
}
public class Quirk_Cursed : Quirk
{
    new public readonly QuirkData_Cursed data;
    public Quirk_Cursed(QuirkData_Cursed data) : base(data)
    {
        this.data = data;
    }
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as ICurseable).cursed = true;
    }
    public override LangText Describe()
    {
        return (target as ICurseable).curseEffectDesc;
    }
}