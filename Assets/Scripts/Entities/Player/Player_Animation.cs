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

    [SerializeField] float walkMoveMag = 0.7f, runMoveMag = 1.0f;
    [SerializeField] float magnitudeControlSpeed = 5.0f;

    [Header("Clips")]
    [SerializeField] AnimationClip itemPickClip;
    [SerializeField] AnimationClip hurtClip;

    Player origin;
    new ControllerClipBlendSystem animation;

    ControlElement canMoveAnimationControl;

    ControlMachine<Animator, int> onlyArmsLayerControl;
    ControlElement<Animator, int> unheldOnlyArmsLayerControl;
    public void OnAwake()
    {
        //anim = GetComponent<Animator>();
        origin = GetComponent<Player>();
        origin.hpComp.onDamage += OnDamage;

        canMoveAnimationControl = new ControlElement(1, () => origin.movement.canMove = false, false);
        origin.movement.canMoveControl.AddControls(canMoveAnimationControl);
    }
    public void OnStart()
    {
        animation = new ControllerClipBlendSystem(anim, controller);
        anim.Rebind();
        AssignMeleeCallbacks();
        InitOnlyArmsLayerControl();

        origin.inventory.onEquippedItemChange += OnEquippedItemChange;
        OnEquippedItemChange(origin.inventory.equipped);
    }
    void OnEquippedItemChange(Item item)
    {
        if (item == null)
        {
            anim.SetFloat(animationTypeID, (int)AnimationType.None);
            unheldOnlyArmsLayerControl.enabled = true;
        }
        else
        {
            anim.SetFloat(animationTypeID, (int)item.heldAnimation);
            unheldOnlyArmsLayerControl.enabled = item.heldAnimation == AnimationType.None;
        }
    }
    void OnDamage(DamageReceivedInfo info)
    {
        if (!info.dontPlayMotion) animation.PlayOneShot(hurtClip, 0.0f, 0.1f);
    }
    readonly int animationTypeID = Animator.StringToHash("AnimationType");
    readonly int movingID = Animator.StringToHash("Moving");
    readonly int runningID = Animator.StringToHash("Running");
    readonly int dashingID = Animator.StringToHash("Dashing");
    readonly int slidingID = Animator.StringToHash("Sliding");
    readonly int moveXID = Animator.StringToHash("moveX");
    readonly int moveYID = Animator.StringToHash("moveY");
    readonly int canMoveID = Animator.StringToHash("CanMove");
    readonly int moveMagnitudeID = Animator.StringToHash("InputMagnitude");
    float magValue = 0.0f;
    public void OnUpdate()
    {
        canMoveAnimationControl.enabled = !anim.GetBool(canMoveID);

        anim.SetBool(movingID, origin.movement.moving);
        anim.SetBool(runningID, origin.movement.running);
        anim.SetBool(dashingID, origin.movement.dashing);
        anim.SetBool(slidingID, origin.movement.sliding);
        anim.SetFloat(moveXID, origin.movement.relativeMoveInput.x);
        anim.SetFloat(moveYID, origin.movement.relativeMoveInput.z);

        if (origin.movement.moving)
        {
            if (origin.movement.running)
            {
                magValue = Mathf.Min(magValue + Time.deltaTime * magnitudeControlSpeed, runMoveMag);
            }
            else
            {
                if (magValue > walkMoveMag) magValue = Mathf.Max(magValue - Time.deltaTime * magnitudeControlSpeed, walkMoveMag);
                else magValue = Mathf.Min(magValue + Time.deltaTime * magnitudeControlSpeed, walkMoveMag);
            }
        }
        else
        {
            magValue = Mathf.Max(magValue - Time.deltaTime * magnitudeControlSpeed, 0.0f);
        }
        anim.SetFloat(moveMagnitudeID, magValue);
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
    void InitOnlyArmsLayerControl()
    {
        onlyArmsLayerControl = new ControlMachine<Animator, int>(anim, 4, (anim, layer) => anim.SetLayerWeight(layer, 1.0f));
        Debug.Log("ADDedeeee");
        foreach (var i in anim.GetBehaviours<LayerWeightControlBehaviour>())
        {
            Debug.Log("ADDed");
            onlyArmsLayerControl.AddControls(i.GetControl());
        }
        unheldOnlyArmsLayerControl = new ControlElement<Animator, int>(1, (anim, layer) => anim.SetLayerWeight(layer, 0.0f));
        onlyArmsLayerControl.AddControls(unheldOnlyArmsLayerControl);
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