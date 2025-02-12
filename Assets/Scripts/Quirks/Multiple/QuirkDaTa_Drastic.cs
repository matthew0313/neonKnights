using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Drastic", menuName = "Scriptables/Quirks/Multiple/Drastic", order = 1)]
public class QuirkData_Drastic : QuirkData
{
    [SerializeField] float m_healthThreshold = 30.0f;
    [SerializeField] int m_shotCountBonus = 1;
    public float healthThreshold => m_healthThreshold;
    public int shotCountBonus => m_shotCountBonus;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target);
    }
    public override Quirk Create()
    {
        return new Quirk_Drastic(this);
    }
}

public class Quirk_Drastic : Quirk
{
    new public readonly QuirkData_Drastic data;
    private Item item;
    public Quirk_Drastic(QuirkData_Drastic data) : base(data)
    {
        this.data = data;
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
        if (item is GunItem gun && gun.owner != null)
        {
            Player player = (Player)(object)gun.owner;
            if (player != null)
            {
                float healthRatio = player.hpComp.hp / player.hpComp.maxHp;
                if (healthRatio <= data.healthThreshold / 100f)
                {
                    fire.fireInfo.shotCount += data.shotCountBonus;
                }
            }
        }
    }

    public override void SetRandom()
    {
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"+{data.shotCountBonus} shot when HP is below {data.healthThreshold}%",
            kr = $"체력이 {data.healthThreshold}% 이하일 때 샷 카운트 +{data.shotCountBonus}"
        };
    }
    public override void Save(DataUnit dataUnit)
    {
        base.Save(dataUnit);
    }
    public override void Load(DataUnit dataUnit)
    {
        base.Load(dataUnit);
    }
    public override Quirk Copy()
    {
        Quirk_Drastic tmp = base.Copy() as Quirk_Drastic;
        return tmp;
    }
}
