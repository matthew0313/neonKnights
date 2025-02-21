using System;
using UnityEngine;
using UnityEngine.Timeline;

public class PlaySoundBehaviour : StateMachineBehaviour
{
    [SerializeField] Sound played;
    [SerializeField] bool loop = false;
    [SerializeField] string soundID = string.Empty;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayingAudio tmp = AudioManager.Instance.PlaySound(played).SetID(soundID);
        if (loop) tmp.Loop();
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