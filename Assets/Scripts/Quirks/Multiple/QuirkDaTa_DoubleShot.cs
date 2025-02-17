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
    
    //Double Shot은 총기류에만 추가할 수 있는 쿼크. IsAddable은 이 '쿼크 추가' 기능이 있는 아이템에 쿼크를 추가할 수 있는지
    //참 거짓을 반환하는 역할을 한다. target이 GunItem일 경우에만 참을 반환해야 한다.
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is GunItem;
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

    //참고: IsAddable에서 GunItem일 경우에만 추가가 가능하므로 이 아이템은 이미 GunItem이라는 보증이 있다.
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        if (target is Item tmp)
        {
            item = tmp;
            if (item is GunItem gun)
            {
                //gun.onFireStart += OnFireStart;
                gun.onFireEnd += OnFireEnd;
            }
        }
    }
    
    //Double Shot은 말 그대로 발사를 2회 하는 쿼크이다. 그저 shotCount를 한 번 늘릴 경우 그냥 샷건이 되고,
    //샷건의 경우에는 그냥 탄환 1개를 더 발사하는 꼴이 된다.

    //참고: GunFireInfo를 수정하고 싶다면 gun.modifyFireInfo에 함수를 붙이면 된다. onFireStart는 한 번의 발사가 시작되었다는 걸
    //알리는 역할을 하고, 이미 GunFireInfo가 수정이 완료되었다는 가정 하에 쿼크들이 이를 확인하고 행동을 할 수 있도록 설계되어 있으므로,
    //onFireStart에서 GunFireInfo를 수정하는 건 바람직하지 않다.

    /*
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
    */

    //이 발사가 끝나고, 확률적으로 동일한 fireInfo를 기반으로 '쿼크에 의해 발사된' 발사를 한다.
    void OnFireEnd(GunFire fire)
    {
        if (fire.fireInfo.source == GunFireSource.GunFire && Random.Range(0f, 100f) <= extraShotChance)
        {
            GunFireInfo info = fire.fireInfo;

            //이 발사가 쿼크에 의한 것인 것으로 설정.
            info.source = GunFireSource.QuirkTriggered;

            //동일한 발사 정보를 가지고 다시 발사.
            (target as GunItem).Fire(info);
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
