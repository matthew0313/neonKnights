using JetBrains.Annotations;
using System;
using UnityEngine;

public class LayerWeightControlBehaviour : StateMachineBehaviour
{
    [SerializeField] int priority = 0;
    [SerializeField] float weight = 0.0f;
    ControlElement<Animator, int> control;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (control != null) control.enabled = true;
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(control != null) control.enabled = false;
    }
    public ControlElement<Animator, int> GetControl()
    {
        if (control == null) control = new ControlElement<Animator, int>(priority, (anim, layer) => anim.SetLayerWeight(layer, weight), false);
        return control;
    }

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
