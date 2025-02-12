using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class Player_Animation : MonoBehaviour
{
    Animator anim;
    [SerializeField] RuntimeAnimatorController controller;

    [Header("Clips")]
    [SerializeField] AnimationClip itemPickClip;

    Player origin;
    new ControllerClipBlendSystem animation;
    public void OnAwake()
    {
        anim = GetComponent<Animator>();
        origin = GetComponent<Player>();
        origin.inventory.onEquippedItemChange += OnEquippedItemChange;
    }
    public void OnStart()
    {
        animation = new ControllerClipBlendSystem(anim, controller);
        anim.Rebind();
        AssignMeleeCallbacks();
    }
    void OnEquippedItemChange(Item item)
    {
        if (item == null) anim.SetInteger(animationTypeID, (int)AnimationType.None);
        else anim.SetInteger(animationTypeID, (int)item.heldAnimation);
    }
    readonly int animationTypeID = Animator.StringToHash("AnimationType");
    readonly int movingID = Animator.StringToHash("Moving");
    readonly int moveXID = Animator.StringToHash("MoveX");
    readonly int moveYID = Animator.StringToHash("MoveY");
    public void OnUpdate()
    {
        anim.SetBool(movingID, origin.movement.relativeMoveInput.magnitude > 0.1f);
        anim.SetFloat(moveXID, origin.movement.relativeMoveInput.x);
        anim.SetFloat(moveYID, origin.movement.relativeMoveInput.z);
    }
    readonly int fireID = Animator.StringToHash("Fire");
    public void TriggerFire()
    {
        animation.GetController().SetTrigger(fireID);
    }
    readonly int reloadID = Animator.StringToHash("Reload");
    public void TriggerReload()
    {
        animation.GetController().SetTrigger(reloadID);
    }
    readonly int drinkStartID = Animator.StringToHash("DrinkStart"), drinkEndID = Animator.StringToHash("DrinkEnd");
    public void TriggerDrinkStart()
    {
        animation.GetController().SetTrigger(drinkStartID);
    }
    public void TriggerDrinkEnd()
    {
        animation.GetController().SetTrigger(drinkEndID);
    }
    readonly int throwID = Animator.StringToHash("Throw");
    public void TriggerThrow()
    {
        animation.GetController().SetTrigger(throwID);
    }
    readonly int slashID = Animator.StringToHash("Slash");
    readonly int comboID = Animator.StringToHash("Combo");
    public Action onSlashStart, onComboEnd;
    void AssignMeleeCallbacks()
    {
        foreach(var i in anim.GetBehaviours<MeleeSlashBehaviour>())
        {
            i.onSlashStart += () => { onSlashStart?.Invoke(); };
        }
        foreach(var i in anim.GetBehaviours<MeleeComboEndBehaviour>())
        {
            i.onComboEnd += () => { onComboEnd?.Invoke(); };
        }
    }
    public void TriggerSlash()
    {
        animation.GetController().SetTrigger(slashID);
    }
    public int GetComboIndex()
    {
        return animation.GetController().GetInteger(comboID);
    }
    public void Play_ItemPick()
    {
        animation.PlayOneShot(itemPickClip);
    }
    private void OnDestroy()
    {
        animation.Destroy();
    }
}
public enum AnimationType
{
    None = 0,
    Pistol = 1,
    SubmachineGun = 2,
    SniperRifle = 3,
    Shotgun = 4,
    Potion = 5,
    Throwable = 6,
    Knife = 7,
    Sword = 8
}