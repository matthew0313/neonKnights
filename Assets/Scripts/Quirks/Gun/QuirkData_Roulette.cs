using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Roulette", menuName = "Scriptables/Quirks/Guns/Roulette", order = 0)]
public class QuirkData_Roulette : QuirkData
{
    [Header("Roulette")]
    [SerializeField] float m_minRouletteRange;
    [SerializeField] float m_maxRouletteRange;
    [SerializeField] float m_minRouletteInterval, m_maxRouletteInterval;
    [SerializeField] Color m_negativeEffectTextColor, m_positiveEffectTextColor;
    public float minRouletteRange => m_minRouletteRange;
    public float maxRouletteRange => m_maxRouletteRange;
    public float minRouletteInterval => m_minRouletteInterval;
    public float maxRouletteInterval => m_maxRouletteInterval;
    public Color negativeEffectTextColor => m_negativeEffectTextColor;
    public Color positiveEffectTextColor => m_positiveEffectTextColor;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is GunItem;
    }
    public override Quirk Create()
    {
        return new Quirk_Roulette(this);
    }
}
public class Quirk_Roulette : Quirk
{
    new public readonly QuirkData_Roulette data;
    public Quirk_Roulette(QuirkData_Roulette data) : base(data)
    {
        this.data = data;
    }
    float rouletteRange, rouletteInterval;
    float currentBonus = 0.0f;
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as GunItem).modifyShotDamageGiven += ModifyShotDamageGiven;
        (target as GunItem).onWield += OnWield;
        (target as GunItem).onWieldUpdate += OnWieldUpdate;
    }
    float counter = 0.0f;
    void ModifyShotDamageGiven(GunShot shot, GunGiveDamageInfo info)
    {
        info.damage.amount *= (100.0f + currentBonus) / 100.0f;
    }
    void OnWield(Player wielder)
    {
        Roulette();
        counter = 0.0f;
    }
    void OnWieldUpdate()
    {
        if (counter < rouletteInterval) counter += Time.deltaTime;
        else
        {
            Roulette();
            counter = 0.0f;
        }
    }
    void Roulette()
    {
        currentBonus = UnityEngine.Random.Range(-rouletteRange, rouletteRange);
        TextIndicator.Indicate((target as GunItem).wielder.transform.position, new LangText()
        {
            en = $"Roulette {Mathf.Round(currentBonus)}% damage",
            kr = $"룰렛 {Mathf.Round(currentBonus)}% 대미지"
        }.text, currentBonus > 0.0f ? data.positiveEffectTextColor : data.negativeEffectTextColor);
    }
    public override void SetRandom()
    {
        base.SetRandom();
        rouletteRange = Mathf.Round(UnityEngine.Random.Range(data.minRouletteRange, data.maxRouletteRange));
        rouletteInterval = Mathf.Round(UnityEngine.Random.Range(data.minRouletteInterval, data.maxRouletteInterval) * 10.0f) / 10.0f;
    }
    public override LangText quirkName => new()
    {
        en = "Roulette",
        kr = "룰렛"
    };
    public override LangText quirkDescription => new()
    {
        en = "Randomly changes damage",
        kr = "대미지를 랜덤하게 변경합니다."
    };
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"-{rouletteRange}%~+{rouletteRange}% damage, {rouletteInterval}s interval",
            kr = $"-{rouletteRange}%~+{rouletteRange}% 대미지, {rouletteInterval}초 주기"
        };
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats["rouletteRange"] = rouletteRange;
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        data.floats.TryGetValue("rouletteRange", out rouletteRange);
    }
    public override Quirk Copy()
    {
        Quirk_Roulette tmp = base.Copy() as Quirk_Roulette;
        tmp.rouletteRange = rouletteRange;
        tmp.rouletteInterval = rouletteInterval;
        return tmp;
    }
}