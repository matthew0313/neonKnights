using System;
using UnityEngine;
using UnityEngine.Timeline;

public class ConversationOpenBehaviour : StateMachineBehaviour
{
    [SerializeField] ConversationSegment segment;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CutsceneManager.Instance.OpenConversation(segment);
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
[System.Serializable]
public class ConversationSegment
{
    [SerializeField] string m_talker;
    [SerializeField][TextArea] string m_text;
    public string talker => m_talker;
    public string text => m_text;
}
