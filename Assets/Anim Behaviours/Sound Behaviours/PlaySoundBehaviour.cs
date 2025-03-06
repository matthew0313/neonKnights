using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PlaySoundBehaviour : StateMachineBehaviour
{
    [SerializeField] Sound played;
    [SerializeField] bool loop = false;
    [SerializeField] string soundID = string.Empty;
    [SerializeField] FloatHook[] volumeHooks;

    [Header("FadeIn")]
    [SerializeField] bool fadeIn = false;
    [SerializeField] float fadeInDuration = 1.0f;
    CoroutineHandle fading;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayingAudio tmp = AudioManager.Instance.PlaySound(played).SetID(soundID);
        if (loop) tmp.Loop();
        foreach (var i in volumeHooks) tmp.HookVolume(i);

        if (fadeIn) fading = Timing.RunCoroutine(FadingIn(tmp));
    }
    IEnumerator<float> FadingIn(PlayingAudio audio)
    {
        audio.volume = 0.0f;
        float counter = 0.0f;
        while (counter < fadeInDuration)
        {
            yield return Timing.WaitForOneFrame;
            counter += Time.deltaTime;
            audio.volume = counter / fadeInDuration;
        }
        audio.volume = 1.0f;
    }
    private void OnDestroy()
    {
        Timing.KillCoroutines(fading);
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