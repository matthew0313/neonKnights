using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Automatic", menuName = "Scriptables/Quirks/Guns/Automatic", order = 0)]
public class QuirkData_Automatic : QuirkData
{
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is GunItem && (target as GunItem).auto == false;
    }
    public override Quirk Create()
    {
        return new Quirk_Automatic(this);
    }
}
public class Quirk_Automatic : Quirk
{
    new public readonly QuirkData_Automatic data;
    public Quirk_Automatic(QuirkData_Automatic data) : base(data)
    {
        this.data = data;
    }
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as GunItem).auto = true;
    }
    public override LangText quirkName => new LangText()
    {
        en = "Automatic",
        kr = "자동"
    };
    public override LangText quirkDescription => new LangText()
    {
        en = "Less stress on your fingers!",
        kr = "손가락에 가는 부담을 덜어줍니다!"
    };
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"Firing mode to Automatic",
            kr = $"발사 모드를 자동으로 전환"
        };
    }
}