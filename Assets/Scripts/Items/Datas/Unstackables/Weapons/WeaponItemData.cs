using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class WeaponItemData : UnstackableItemData
{
    [Header("Weapon")]
    [SerializeField] Weapon m_prefab;
    public Weapon prefab => m_prefab;
}
public abstract class WeaponItem : UnstackableItem, IQuirkable, IQuirkDisplayed
{
    Weapon m_weapon = null;
    public Weapon weapon
    {
        get
        {
            if(m_weapon == null)
            {
                m_weapon = MonoBehaviour.Instantiate(data.prefab);
                m_weapon.gameObject.SetActive(false);
            }
            return m_weapon;
        }
    }

    new public readonly WeaponItemData data;
    public WeaponItem(WeaponItemData data) : base(data)
    {
        this.data = data;
    }
    public override RotateMode rotateMode => RotateMode.CameraLocked;
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        weapon.transform.SetParent(wielder.inventory.equipAnchor, false);
        weapon.gameObject.SetActive(true);
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        weapon.gameObject.SetActive(false);
    }

    public List<Quirk> quirks { get; } = new();
    public override void Save(DataUnit data)
    {
        base.Save(data);
        QuirkSaveList quirkSaves = new();
        foreach(var i in quirks)
        {
            QuirkSave tmp = new();
            tmp.data = i.data;
            i.Save(tmp.save);
            quirkSaves.list.Add(tmp);
        }
        data.strings["quirks"] = JsonUtility.ToJson(quirkSaves);
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        if(data.strings.TryGetValue("quirks", out string tmp))
        {
            QuirkSaveList quirkSaves = JsonUtility.FromJson<QuirkSaveList>(tmp);
            if(quirkSaves != null)
            {
                foreach (var i in quirkSaves.list)
                {
                    if (i.data == null) continue;
                    Quirk quirk = i.data.Create();
                    quirk.Load(i.save);
                    (this as IQuirkable).AddQuirk(quirk);
                }
            }
        }
    }
    public override Item Copy()
    {
        WeaponItem tmp = base.Copy() as WeaponItem;
        foreach(var i in quirks)
        {
            (tmp as IQuirkable).AddQuirk(i.Copy());
        }
        return tmp;
    }
    public override void Delete()
    {
        base.Delete();
        if(m_weapon != null)
        {
            MonoBehaviour.Destroy(m_weapon);
        }
    }
    public IEnumerable<Quirk> GetQuirks()
    {
        foreach (var i in quirks) yield return i;
    }
}
[System.Serializable]
public class QuirkSaveList
{
    public List<QuirkSave> list = new();
}
[System.Serializable]
public class QuirkSave
{
    public QuirkData data;
    public DataUnit save = new();
}