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
public abstract class WeaponItem : UnstackableItem
{
    public abstract HeldWeapon animationType { get; }
    public readonly Weapon weapon;

    new public readonly WeaponItemData data;
    public WeaponItem(WeaponItemData data) : base(data)
    {
        this.data = data;
        weapon = MonoBehaviour.Instantiate(data.prefab);
        weapon.gameObject.SetActive(false);
    }
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        wielder.movement.rotateMode = RotateMode.MouseLocked;
        weapon.transform.SetParent(wielder.inventory.equipAnchor, false);
        weapon.gameObject.SetActive(true);
    }
    public override void OnWieldUpdate()
    {
        base.OnWieldUpdate();
        foreach (var i in quirks) i.OnQuirkUpdate(this);
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        wielder.movement.rotateMode = RotateMode.Free;
        weapon.gameObject.SetActive(false);
    }

    List<Quirk> quirks = new();
    public void AddQuirk(Quirk quirk)
    {
        quirks.Add(quirk);
        quirk.OnQuirkAdd(this);
    }
    public bool FindQuirk<T>(out T quirk) where T : Quirk
    {
        if (FindQuirk((tmp) => tmp is T, out Quirk tmp))
        {
            quirk = tmp as T;
            return true;
        }
        else
        {
            quirk = null;
            return false;
        }
    }
    public bool FindQuirk(Func<Quirk, bool> predicate, out Quirk quirk)
    {
        foreach(var i in quirks)
        {
            if (predicate.Invoke(i))
            {
                quirk = i;
                return true;
            }
        }
        quirk = null;
        return false;
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        List<QuirkSave> quirkSaves = new();
        foreach(var i in quirks)
        {
            QuirkSave tmp = new();
            tmp.data = i.data;
            i.Save(tmp.save);
        }
        data.strings["quirks"] = JsonUtility.ToJson(quirkSaves);
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        if(data.strings.TryGetValue("quirks", out string tmp))
        {
            List<QuirkSave> quirkSaves = JsonUtility.FromJson<List<QuirkSave>>(tmp);
            foreach(var i in quirkSaves)
            {
                Quirk quirk = i.data.Create();
                quirk.Load(i.save);
                AddQuirk(quirk);
            }
        }
    }
}
[System.Serializable]
public class QuirkSave
{
    public QuirkData data;
    public DataUnit save = new();
}
[System.Serializable]
public enum WeaponType
{
    None = 0,
    Pistol = 1
}