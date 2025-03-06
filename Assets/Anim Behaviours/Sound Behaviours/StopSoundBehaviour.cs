using System;
using UnityEngine;
using UnityEngine.Timeline;
using MEC;
using System.Collections.Generic;

public class StopSoundBehaviour : StateMachineBehaviour
{
    [SerializeField] string soundID = string.Empty;

    [Header("Fadeout")]
    [SerializeField] bool fadeOut = false;
    [SerializeField] float fadeOutDuration = 1.0f;
    CoroutineHandle fading;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayingAudio tmp = AudioManager.Instance.GetPlayingAudio(soundID);
        if (tmp == null) return;

        if (!fadeOut) tmp.Stop();
        else fading = Timing.RunCoroutine(FadingOut(tmp));
    }
    IEnumerator<float> FadingOut(PlayingAudio audio)
    {
        float startVolume = audio.volume;
        float counter = 0.0f;
        while(counter < fadeOutDuration)
        {
            yield return Timing.WaitForOneFrame;
            counter += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVolume, 0.0f, counter / fadeOutDuration);
        }
        audio.Stop();
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