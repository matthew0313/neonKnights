using System;
using UnityEngine;
using UnityEngine.Timeline;

public class TimelinePlayBehaviour : StateMachineBehaviour
{
    [SerializeField] TimelineAsset timeline;
    [SerializeField] bool progressOnFinish = false;
    readonly int progressID = Animator.StringToHash("Progress");
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CutsceneManager.Instance.PlayTimeline(timeline, () =>
        {
            if (progressOnFinish) animator.SetTrigger(progressID);
        });
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
