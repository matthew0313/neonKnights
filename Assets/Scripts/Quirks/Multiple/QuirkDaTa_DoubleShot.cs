using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_DoubleShot", menuName = "Scriptables/Quirks/Multiple/DoubleShot", order = 1)]
public class QuirkData_DoubleShot : QuirkData
{
    [SerializeField] float m_minExtraShotChance = 25.0f;
    [SerializeField] float m_maxExtraShotChance = 25.0f;
    
    public float minExtraShotChance => m_minExtraShotChance;
    public float maxExtraShotChance => m_maxExtraShotChance;
    
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target);
    }
    
    public override Quirk Create()
    {
        return new Quirk_DoubleShot(this);
    }
}

public class Quirk_DoubleShot : Quirk
{
    new public readonly QuirkData_DoubleShot data;
    private float extraShotChance;
    private Item item;
    
    public Quirk_DoubleShot(QuirkData_DoubleShot data) : base(data)
    {
        this.data = data;
        extraShotChance = data.minExtraShotChance;
    }
    
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        if (target is Item tmp)
        {
            item = tmp;
            if (item is GunItem gun)
            {
                gun.onFireStart += OnFireStart;
            }
        }
    }
    
    void OnFireStart(GunFire fire)
    {
        if (item is GunItem gun)
        {
            if (Random.Range(0f, 100f) <= extraShotChance)
            {
                fire.fireInfo.shotCount += 1;
            }
        }
    }
    
    public override void SetRandom()
    {
        extraShotChance = Mathf.Round(Random.Range(data.minExtraShotChance, data.maxExtraShotChance) * 10f) / 10f;
    }
    
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"Chance for an extra shot: {extraShotChance}%",
            kr = $"추가 샷 확률: {extraShotChance}%"
        };
    }
    
    public override void Save(DataUnit dataUnit)
    {
        base.Save(dataUnit);
        dataUnit.floats["extraShotChance"] = extraShotChance;
    }
    
    public override void Load(DataUnit dataUnit)
    {
        base.Load(dataUnit);
        dataUnit.floats.TryGetValue("extraShotChance", out extraShotChance);
    }
    
    public override Quirk Copy()
    {
        Quirk_DoubleShot tmp = base.Copy() as Quirk_DoubleShot;
        tmp.extraShotChance = extraShotChance;
        return tmp;
    }
}
