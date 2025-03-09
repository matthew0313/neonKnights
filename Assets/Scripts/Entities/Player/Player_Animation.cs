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

    [SerializeField] float walkMoveMag = 0.7f, runMoveMag = 1.0f;
    [SerializeField] float magnitudeControlSpeed = 5.0f;

    [SerializeField] private float animationSmoothTime = 0.1f;
    
    private Animator animator;
    private int moveXHash;
    private int moveYHash;

    private int inputMagnitudeHash;

    Player origin;
    new ControllerClipBlendSystem animation;

    ControlElement canMoveAnimationControl;
    public void OnAwake()
    {
        anim = GetComponent<Animator>();
        origin = GetComponent<Player>();
        origin.inventory.onEquippedItemChange += OnEquippedItemChange;
        origin.hpComp.onDamage += OnDamage;

        canMoveAnimationControl = new ControlElement(1, () => origin.movement.canMove = false, false);
        origin.movement.canMoveControl.AddControls(canMoveAnimationControl);
    }
    public void OnStart()
    {
        //animation = new ControllerClipBlendSystem(anim, controller);
        animator = GetComponent<Animator>();
        anim.Rebind();
        AssignMeleeCallbacks();
        OnEquippedItemChange(origin.inventory.equipped);
        moveXHash = Animator.StringToHash("moveX");
        moveYHash = Animator.StringToHash("moveY");

        inputMagnitudeHash = Animator.StringToHash("InputMagnitude");
    }

    public void UpdateInputMagniude(Vector2 inputVector)
    {
        if (animator == null) return;

        float magnitude = inputVector.magnitude;
        float currentMagnitude = animator.GetFloat(inputMagnitudeHash);
        float newMagnitude = Mathf.Lerp(currentMagnitude, magnitude, Time.deltaTime / animationSmoothTime);
        animator.SetFloat(inputMagnitudeHash, newMagnitude); 
    }

    void OnEquippedItemChange(Item item)
    {
        if (item == null) anim.SetFloat(animationTypeID, (int)AnimationType.None);
        else anim.SetFloat(animationTypeID, (int)item.heldAnimation);
    }
    //void OnEquippedItemChange(Item item)
    //{
        //if (item == null) 
            //anim.SetFloat(animationTypeID, AnimationType.None); 
        //else 
            //anim.SetFloat(animationTypeID, item.heldAnimation);
    //}
    void OnDamage(DamageReceivedInfo info)
    {
        if (!info.dontPlayMotion) animation.PlayOneShot(hurtClip);
    }
    readonly int animationTypeID = Animator.StringToHash("AnimationType");
    readonly int movingID = Animator.StringToHash("Moving");
    readonly int runningID = Animator.StringToHash("Running");
    readonly int dashingID = Animator.StringToHash("Dashing");
    readonly int slidingID = Animator.StringToHash("Sliding");
    //readonly int moveXID = Animator.StringToHash("MoveX");
    //readonly int moveYID = Animator.StringToHash("MoveY");
    readonly int canMoveID = Animator.StringToHash("CanMove");
    readonly int moveMagnitudeID = Animator.StringToHash("moveMagnitude");
    readonly int aimingID = Animator.StringToHash("IsAiming");
public void SetAiming(bool aiming)
{
    anim.SetBool(aimingID, aiming);
}
float magValue = 0.0f;
public void OnUpdate()
{
    canMoveAnimationControl.enabled = !anim.GetBool(canMoveID);

    anim.SetBool(movingID, origin.movement.moving);
    anim.SetBool(runningID, origin.movement.running);
    anim.SetBool(dashingID, origin.movement.dashing);
    anim.SetBool(slidingID, origin.movement.sliding);
    //anim.SetFloat(moveXID, origin.movement.relativeMoveInput.x);
    //anim.SetFloat(moveYID, origin.movement.relativeMoveInput.z);

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
    void Update()
    {
        UpdateAnimatorParameters(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    void UpdateAnimatorParameters(float moveX, float moveY)
    {
        if (animator == null)
        {
            Debug.LogError("Animator가 null");
            return;
        }
        
        Vector2 movementVector = new Vector2(moveX, moveY);

        if (movementVector.magnitude > 1f)
        {
            movementVector.Normalize();
        }

        float currentMoveX = animator.GetFloat(moveXHash);
        float currentMoveY = animator.GetFloat(moveYHash);
        float newMoveX = Mathf.Lerp(currentMoveX, movementVector.x, Time.deltaTime/animationSmoothTime);
        float newMoveY = Mathf.Lerp(currentMoveY, movementVector.y, Time.deltaTime/animationSmoothTime);
        animator.SetFloat(moveXHash, newMoveX);
        animator.SetFloat(moveYHash, newMoveY);
        Debug.Log($"설정 후: moveX={animator.GetFloat(moveXHash)}, moveY={animator.GetFloat(moveYHash)}");
    
        Debug.Log("{moveX},{moveY} | {currentMoveX},{currentMoveY} | {newMoveX},{newMoveY}");
  
    }

    public void SetMovementInput(float moveX, float moveY)
    {
        UpdateAnimatorParameters(moveX, moveY);
    }

    public void SetMovementInput(Vector2 movement)
    {
        SetMovementInput(movement.x, movement.y);
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
//public static class AnimationType
//{
    //public const float None = 0f;
    //public const float Pistol = 1f;
    //public const float SubmachineGun = 2f;
    //public const float SniperRifle = 3f;
    //public const float Shotgun = 4f;
    //public const float Potion = 5f;
    //public const float Throwable = 6f;
    //public const float Knife = 7f;
    //public const float Sword = 8f;
    //public const float Axe = 9f;
//}