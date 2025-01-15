using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Player_Animation : MonoBehaviour
{
    internal Animator anim;
    Player origin;
    public void OnAwake()
    {
        anim = GetComponent<Animator>();
        origin = GetComponent<Player>();
    }
    readonly int animationTypeID = Animator.StringToHash("AnimationType");
    readonly int movingID = Animator.StringToHash("Moving");
    readonly int moveXID = Animator.StringToHash("MoveX");
    readonly int moveYID = Animator.StringToHash("MoveY");
    public void SetAnimationType(AnimationType type)
    {
        anim.SetInteger(animationTypeID, (int)type);
    }
    public void OnUpdate()
    {
        anim.SetBool(movingID, origin.movement.relativeMoveInput.magnitude > 0.1f);
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
}
public enum AnimationType
{
    None = 0,
    Pistol = 1
}