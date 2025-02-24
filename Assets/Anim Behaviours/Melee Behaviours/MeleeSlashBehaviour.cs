using System;
using UnityEngine;

public class MeleeSlashBehaviour : StateMachineBehaviour
{
    [Header("Slash")]
    [SerializeField] int comboIndex = 0;
    [SerializeField] float slashStartTime = 0.0f, slashEndTime = 1.0f;

    [Header("Effect")]
    [SerializeField] SlashEffect effect;
    [SerializeField] float effectStartInterval;
    [SerializeField] Vector3 pos, rot;
    Transform effectAnchor;

    public Action<int> onSlashStart;
    public Action<int> onSlashEnd;
    SlashState state;

    bool effectPlayed = false;
    float counter = 0.0f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(effectAnchor == null)
        {
            if (animator.TryGetComponent(out SlashEffectAnchorLocator locator))
            {
                effectAnchor = locator.slashEffectAnchor;
            }
            else effectAnchor = animator.transform;
        }
        counter = 0.0f;
        effectPlayed = false;
        state = SlashState.NotStarted;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        counter += Time.deltaTime;
        if (!effectPlayed)
        {
            if (counter >= effectStartInterval)
            {
                effectPlayed = true;
                if (effect != null && effectAnchor != null)
                {
                    SlashEffect tmp = effect.Instantiate();
                    tmp.transform.SetParent(effectAnchor, false);
                    tmp.transform.localPosition = pos;
                    tmp.transform.localRotation = Quaternion.Euler(rot);
                }
            }
        }

        switch (state)
        {
            case SlashState.NotStarted:
                if(counter >= slashStartTime)
                {
                    state = SlashState.Slashing;
                    onSlashStart?.Invoke(comboIndex);
                }
                break;
            case SlashState.Slashing:
                if(counter >= slashEndTime)
                {
                    state = SlashState.Ended;
                    onSlashEnd?.Invoke(comboIndex);
                }
                break;
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(state == SlashState.Slashing)
        {
            onSlashEnd?.Invoke(comboIndex);
        }
    }
    enum SlashState
    {
        NotStarted,
        Slashing,
        Ended
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
