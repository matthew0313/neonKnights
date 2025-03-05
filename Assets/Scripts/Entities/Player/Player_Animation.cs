using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine;
using UnityEngine.Playables;

public class Player_Animation : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] RuntimeAnimatorController controller;

    [Header("Clips")]
    [SerializeField] AnimationClip itemPickClip;
    [SerializeField] AnimationClip hurtClip;

    Player origin;
    new ControllerClipBlendSystem animation;

    ControlElement canMoveAnimationControl;
    public void OnAwake()
    {
        //anim = GetComponent<Animator>();
        origin = GetComponent<Player>();
        origin.inventory.onEquippedItemChange += OnEquippedItemChange;
        origin.hpComp.onDamage += OnDamage;

        canMoveAnimationControl = new ControlElement(1, () => origin.movement.canMove = false, false);
        origin.movement.canMoveControl.AddControls(canMoveAnimationControl);
    }
    public void OnStart()
    {
        animation = new ControllerClipBlendSystem(anim, controller);
        anim.Rebind();
        AssignMeleeCallbacks();
        OnEquippedItemChange(origin.inventory.equipped);
    }
    void OnEquippedItemChange(Item item)
    {
        if (item == null) anim.SetInteger(animationTypeID, (int)AnimationType.None);
        else anim.SetInteger(animationTypeID, (int)item.heldAnimation);
    }
    void OnDamage(DamageReceivedInfo info)
    {
        if (!info.dontPlayMotion) animation.PlayOneShot(hurtClip);
    }
    readonly int animationTypeID = Animator.StringToHash("AnimationType");
    readonly int movingID = Animator.StringToHash("Moving");
    readonly int runningID = Animator.StringToHash("Running");
    readonly int dashingID = Animator.StringToHash("Dashing");
    readonly int slidingID = Animator.StringToHash("Sliding");
    readonly int moveXID = Animator.StringToHash("MoveX");
    readonly int moveYID = Animator.StringToHash("MoveY");
    readonly int canMoveID = Animator.StringToHash("CanMove");
    public void OnUpdate()
    {
        canMoveAnimationControl.enabled = !anim.GetBool(canMoveID);

        anim.SetBool(movingID, origin.movement.moving);
        anim.SetBool(runningID, origin.movement.running);
        anim.SetBool(dashingID, origin.movement.dashing);
        anim.SetBool(slidingID, origin.movement.sliding);
        anim.SetFloat(moveXID, origin.movement.relativeMoveInput.x);
        anim.SetFloat(moveYID, origin.movement.relativeMoveInput.z);
    }

    readonly int fireID = Animator.StringToHash("Fire");
    public void TriggerFire()
    {
        anim.SetTrigger(fireID);
    }
    readonly int reloadID = Animator.StringToHash("Reload");
    public void TriggerReload()
    {
        anim.SetTrigger(reloadID);
    }
    readonly int drinkStartID = Animator.StringToHash("DrinkStart"), drinkEndID = Animator.StringToHash("DrinkEnd");
    public void TriggerDrinkStart()
    {
        anim.SetTrigger(drinkStartID);
    }
    public void TriggerDrinkEnd()
    {
        anim.SetTrigger(drinkEndID);
    }
    readonly int throwID = Animator.StringToHash("Throw");
    public void TriggerThrow()
    {
        anim.SetTrigger(throwID);
    }
    readonly int slashID = Animator.StringToHash("Slash");
    public Action<int> onSlashStart;
    public Action<int> onSlashEnd;
    void AssignMeleeCallbacks()
    {
        foreach(var i in anim.GetBehaviours<MeleeSlashBehaviour>())
        {
            i.onSlashStart += (int comboIndex) => onSlashStart?.Invoke(comboIndex);
            i.onSlashEnd += (int comboIndex) => onSlashEnd?.Invoke(comboIndex);
        }
    }
    public void TriggerSlash()
    {
        anim.SetTrigger(slashID);
    }
    public void PlayItemPick()
    {
        animation.PlayOneShot(itemPickClip);
    }
    readonly int aimingID = Animator.StringToHash("IsAiming");
    public void SetAiming(bool aiming)
    {
        anim.SetBool(aimingID, aiming);
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
    Sword = 8,
    Axe = 9
}