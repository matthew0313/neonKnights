using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Player_Animation : MonoBehaviour
{
    internal Animator anim;
    void OnAwake()
    {
        anim = GetComponent<Animator>();
        GetComponent<Player>().inventory.onEquippedItemChange += OnEquippedItemChange;
    }
    readonly int heldWeaponID = Animator.StringToHash("HeldWeapon");
    public void SetHeldWeapon(HeldWeapon weapon)
    {
        anim.SetInteger(heldWeaponID, (int)weapon);
    }
    void OnEquippedItemChange(Item item)
    {
        if(item is WeaponItem)
        {
            anim.SetInteger(heldWeaponID, (int)(item as WeaponItem).animationType);
        }
        else
        {
            anim.SetInteger(heldWeaponID, (int)HeldWeapon.None);
        }
    }
}
public enum HeldWeapon
{
    None = 0,
    Pistol = 1
}