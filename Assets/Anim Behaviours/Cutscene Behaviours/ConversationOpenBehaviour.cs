using System;
using UnityEngine;
using UnityEngine.Timeline;

public class ConversationOpenBehaviour : StateMachineBehaviour
{
    [SerializeField] ConversationSegment segment;
    [SerializeField] bool progress = true;
    readonly int progressID = Animator.StringToHash("Progress");
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CutsceneManager.Instance.OpenConversation(segment, () => { if (progress) animator.SetTrigger(progressID); });
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
    [SerializeField] LangText m_talker;
    [SerializeField] LangText m_text;
    [SerializeField] Sound m_talkSound;
    [SerializeField] bool m_skippable = true;
    public LangText talker => m_talker;
    public LangText text => m_text;
    public Sound talkSound => m_talkSound;
    public bool skippable => m_skippable;
}
