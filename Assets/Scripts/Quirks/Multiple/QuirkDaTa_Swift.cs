using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Swift", menuName = "Scriptables/Quirks/Multiple/Swift", order = 1)]
public class QuirkData_Swift : QuirkData
{
    [Header("Speed Boost")]
    [SerializeField] float m_minSpeedIncrease = 40.0f;
    [SerializeField] float m_maxSpeedIncrease = 40.0f;
    [SerializeField] float m_upgradeSpeedIncreaseIncrease;
    
    public float minSpeedIncrease => m_minSpeedIncrease;
    public float maxSpeedIncrease => m_maxSpeedIncrease;
    public float upgradeSpeedIncreaseIncrease => m_upgradeSpeedIncreaseIncrease;
    
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is WeaponItem;
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
    private Item item;
    
    public Quirk_Swift(QuirkData_Swift data) : base(data)
    {
        this.data = data;
        this.speedIncrease = data.minSpeedIncrease; 
    }
    
    //이러면 총을 들고 있을 때 쿼크가 추가되면 플레이어 이동 속도가 늘어나고, 영원히 그 상태로 유지된다.
    //Swift는 총을 들면 플레이어의 속도가 일정 수준 올라가는 쿼크다.

    //참고: moveSpeed는 플레이어의 기본 속도로, 총 무게에 의해 더해지고 빼지는 값이다.
    //더해지고 빼지는 값에 곱하기, 나누기를 해 버리면 값이 꼬인다.
    //statistics의 speedMultiplier는 이동 속도 배율로, 얘는 곱하고 나누는 용도이다.
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        if (target is Item tmp)
        {
            item = tmp;
            /*if (gunItem.wielder != null && gunItem.wielder.movement != null)
            {
                gunItem.wielder.movement.moveSpeed *= (1.0f + speedIncrease / 100.0f);
            }*/
            item.onWield += OnWield;
            item.onUnwield += OnUnwield;
            if (item.wielder != null) OnWield(item.wielder);
        }
    }
    bool added = false;
    void OnWield(Player wielder)
    {
        if (added) return;
        added = true;
        wielder.statistics.speedMultiplier *= 1.0f + speedIncrease / 100.0f;
    }
    void OnUnwield(Player wielder)
    {
        if (!added) return;
        added = false;
        wielder.statistics.speedMultiplier /= 1.0f + speedIncrease / 100.0f;
    }

    //Spooling이랑 같은 이유.
    /*
    public new void OnQuirkRemove(IQuirkable target)
    {
        if (gunItem != null && gunItem.wielder != null && gunItem.wielder.movement != null)
        {
            gunItem.wielder.movement.moveSpeed /= (1.0f + speedIncrease / 100.0f);
        }
        base.OnQuirkRemove(target);
    }
    */
    

    //Describe에서 speedIncrease값이 20.124313513 등으로 길게 나올 수 있으므로, 소수점 첫 번째 자리까지 반올림한다.
    public override void SetRandom()
    {
        speedIncrease = Mathf.Round(UnityEngine.Random.Range(data.minSpeedIncrease, data.maxSpeedIncrease) * 10.0f) / 10.0f;
    }
    
    //Spooling이랑 같은 이유.
    public override LangText Describe()
    {
        /*return new LangText()
        {
            en = $"Increases movement speed by {speedIncrease}% while holding a gun.",
            kr = $"총을 들고 있을 때 이동 속도가 {speedIncrease}% 증가합니다."
        };*/
        return new LangText()
        {
            en = $"+{speedIncrease}% moveSpeed when held",
            kr = $"들고 있으면 +{speedIncrease}% 속도"
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
    public override bool IsUpgradable()
    {
        return speedIncrease < data.maxSpeedIncrease;
    }
    public override void Upgrade()
    {
        base.Upgrade();
        speedIncrease = Mathf.Min(speedIncrease + data.upgradeSpeedIncreaseIncrease, data.maxSpeedIncrease);
    }
}
