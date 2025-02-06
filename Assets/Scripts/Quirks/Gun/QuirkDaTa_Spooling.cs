using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Spooling", menuName = "Scriptables/Quirks/Guns/Spooling", order = 0)]
public class QuirkData_Spooling : QuirkData
{
    [SerializeField] float reductionPerShot = 0.1f;
    [SerializeField] float minMultiplier = 0.5f;
    [SerializeField] float spoolResetTime = 2.0f;

    public float ReductionPerShot => reductionPerShot;
    public float MinMultiplier => minMultiplier;
    public float SpoolResetTime => spoolResetTime;

    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is GunItem;
    }

    public override Quirk Create()
    {
        return new Quirk_Spooling(this);
    }
}

public class Quirk_Spooling : Quirk
{
    new public readonly QuirkData_Spooling data;
    private GunItem gun;
    private float currentMultiplier;

    //lastShotTime은 Time.time을 사용한다. 이는 저장하고 게임을 끄고 로드하면 바뀌는 값이므로, 남은 시간을 저장하는 역할을 하지 못한다.
    //남은 시간을 저장하고 싶다면, 별도로 남은 시간을 업데이트하고 그 값을 저장/로드한다.
    //originalFireRate의 주석 이유는 하단에 있다.

    //private float lastShotTime;
    //private float originalFireRate;

    private float counter = 0.0f;
    public Quirk_Spooling(QuirkData_Spooling data) : base(data)
    {
        this.data = data;
    }

    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        gun = target as GunItem;
        if (gun != null)
        {
            //originalFireRate = gun.fireRate;
            currentMultiplier = 1.0f;
            //lastShotTime = 0f;
            //gun.onFireStart += OnFireStart;
            gun.modifyFireInfo += ModifyFireInfo;
            gun.onUpdate += OnUpdate;
        }
    }
    //GunItem의 발사 방식은 다음과 같음.
    //1. GunFireInfo를 생성하고, GunFireInfo에 현재 스탯값을 넣는다. (대미지, 발사주기 등)
    //2. 쿼크들이 해당 GunFireInfo를 자신의 현재 조건에 맞춰 수정한다.
    //3. GunItem이 수정된 GunFireInfo를 기반으로 발사를 한다.

    //쿼크가 아이템의 스텟을 직접적으로 건드리는 건 다음과 같은 경우에 해당한다:
    //1. 아무 조건 없이 진짜로 그냥 추가할 때 스탯이 바뀌는 쿼크일 경우

    //이 외의 쿼크는 총을 발사하기 전 생성되는 GunFireInfo를 수정하는 방식으로 총의 스탯에 간섭한다.

    //이렇게 하는 이유는 UI에 표기되는 스탯을 일관성 있게 하기 위해서다.
    //또한, 여러 쿼크가 같은 스탯을 건드리다 보면 자칫하면 총의 스탯이 영구적으로 꼬여 버릴 가능성도 존재한다.

    //이런 이유로 originalFireRate 와 OnFireStart를 주석 처리한다.
    /*
    private void OnFireStart(GunFire fire)
    {
        if (Time.time - lastShotTime > data.SpoolResetTime)
        {
            currentMultiplier = 1.0f;
        }
        else
        {
            currentMultiplier = Mathf.Max(currentMultiplier - data.ReductionPerShot, data.MinMultiplier);
        }
        lastShotTime = Time.time;
        gun.fireRate = originalFireRate * currentMultiplier;
    }
    */
    private void ModifyFireInfo(GunFireInfo info)
    {
        counter = data.SpoolResetTime;
        info.fireRate *= currentMultiplier;
        currentMultiplier = Mathf.Max(currentMultiplier - data.ReductionPerShot, data.MinMultiplier);
    }
    void OnUpdate()
    {
        if(counter > 0.0f)
        {
            counter -= Time.deltaTime;
            if(counter <= 0.0f)
            {
                currentMultiplier = 1.0f;
            }
        }
    }



    //쿼크의 제거 기능은 예정되어 있지 않음. 만일 쿼크 제거 기능을 추가하기로 결정한다면 virtual/override를 이용해 만드는 게 좋음.
    //지금 이 상태의 OnQuirkRemove는 Quirk_Spooling 타입 변수로 저장해야 접근해야 접근이 가능하고, Quirk 타입의 변수에선 접근이 불가하다.
    /*
    public new void OnQuirkRemove(IQuirkable target)
    {
        if (gun != null)
        {
            gun.onFireStart -= OnFireStart;
            gun.fireRate = originalFireRate;
        }
        base.OnQuirkRemove(target);
    }
    */

    //설명이 너무 길다. 이러면 설명 상자를 벗어나 버린다. Describe()는 변경될 수 있는 값을 포함해 기능의 최소한의 요약을 반환한다.
    public override LangText Describe()
    {
        /*
         * return new LangText()
        {
            en = $"Spooling: Continuous shots reduce fire rate to {data.MinMultiplier * 100f}% of base rate.",
            kr = $"스풀링: 연속 사격 시 발사 주기가 기본의 {data.MinMultiplier * 100f}%까지 감소합니다."
        };
        */
        return new LangText()
        {
            en = $"{data.MinMultiplier * 100f}% Firerate on continuous fire",
            kr = $"연속 사격 시 발사주기 {data.MinMultiplier * 100f}%"
        };
    }

    //위에서 originalFireRate를 주석 처리하긴 했지만, 세이브가 로드될 때마다 쿼크는 로딩 후 다시 추가되므로 originalFireRate는 다시 OnQuirkAdd에서 설정된다.
    //그러므로 originalFireRate를 사용한다고 해도 이를 저장하는 것은 의미가 없다.
    public override void Save(DataUnit dataUnit)
    {
        base.Save(dataUnit);
        dataUnit.floats["currentMultiplier"] = currentMultiplier;
        //dataUnit.floats["lastShotTime"] = lastShotTime;
        //dataUnit.floats["originalFireRate"] = originalFireRate;
        dataUnit.floats["counter"] = counter;
    }

    public override void Load(DataUnit dataUnit)
    {
        base.Load(dataUnit);
        dataUnit.floats.TryGetValue("currentMultiplier", out currentMultiplier);
        //dataUnit.floats.TryGetValue("lastShotTime", out lastShotTime);
        //dataUnit.floats.TryGetValue("originalFireRate", out originalFireRate);
        dataUnit.floats.TryGetValue("counter", out counter);
    }

    public override Quirk Copy()
    {
        Quirk_Spooling tmp = base.Copy() as Quirk_Spooling;
        tmp.currentMultiplier = currentMultiplier;
        tmp.counter = counter;
        //tmp.lastShotTime = lastShotTime;
        //tmp.originalFireRate = originalFireRate;
        return tmp;
    }
}
