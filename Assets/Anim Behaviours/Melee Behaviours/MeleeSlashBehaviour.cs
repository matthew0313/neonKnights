using System;
using UnityEngine;

public class MeleeSlashBehaviour : StateMachineBehaviour
{
    Player player;

    [SerializeField] int comboIndex;

    [Header("Effect")]
    [SerializeField] SlashEffect effect;
    [SerializeField] float effectStartInterval;
    [SerializeField] Vector3 pos, rot;

    public Action onSlashStart;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    readonly int comboID = Animator.StringToHash("Combo");

    bool effectPlayed = false;
    float counter = 0.0f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null) animator.TryGetComponent(out player);
        animator.SetInteger(comboID, comboIndex);
        onSlashStart?.Invoke();
        counter = 0.0f;
        effectPlayed = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!effectPlayed)
        {
            counter += Time.deltaTime;
            if (counter >= effectStartInterval)
            {
                effectPlayed = true;
                if (effect != null && player != null)
                {
                    SlashEffect tmp = effect.Instantiate();
                    tmp.transform.SetParent(player.movement.rotator, false);
                    tmp.transform.localPosition = pos;
                    tmp.transform.localRotation = Quaternion.Euler(rot);
                }
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
