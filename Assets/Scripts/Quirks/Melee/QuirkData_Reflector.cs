using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Reflector", menuName = "Scriptables/Quirks/Melees/Reflector", order = 0)]
public class QuirkData_Reflector : QuirkData
{
    [Header("Reflector")]
    [SerializeField] Sound m_reflectSound;
    public Sound reflectSound => m_reflectSound;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is MeleeItem;
    }
    public override Quirk Create()
    {
        return new Quirk_Reflector(this);
    }
}
public class Quirk_Reflector : Quirk
{
    new public readonly QuirkData_Reflector data;
    public Quirk_Reflector(QuirkData_Reflector data) : base(data)
    {
        this.data = data;
    }
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as MeleeItem).onSlashHit += OnSlashHit;
    }
    void OnSlashHit(MeleeSlash slash, GameObject hit)
    {
        if(hit.TryGetComponent(out EnemyBullet bullet))
        {
            bullet.Reflect();
            AudioManager.Instance.PlaySound(data.reflectSound);
        }
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"Reflects enemy bullets",
            kr = $"적 탄환 반사"
        };
    }
}