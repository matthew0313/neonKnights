using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Drastic", menuName = "Scriptables/Quirks/Guns/Drastic", order = 1)]
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
    //플레이어 참조는 gun.wielder. (경고: 총을 들고 있는 도중에만 참조 가능)
    //Character 타입은 글자 타입...
    void OnFireStart(GunFire fire)
    {
        if (item is GunItem gun)
        {
            Player player = gun.wielder;
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
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"+{data.shotCountBonus} shot count below {data.healthThreshold}% HP",
            kr = $"체력이 {data.healthThreshold}% 이하면 샷 카운트 +{data.shotCountBonus}"
        };
    }
}
