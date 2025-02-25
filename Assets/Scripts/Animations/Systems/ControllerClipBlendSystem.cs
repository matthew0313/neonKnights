using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using MEC;
using System;

public class ControllerClipBlendSystem
{
    PlayableGraph playGraph;
    AnimationMixerPlayable topMixer;

    readonly AnimatorControllerPlayable controllerPlayable;

    AnimationClipPlayable oneShotPlayable;
    CoroutineHandle oneShotBlendIn, oneShotBlendOut;
    public ControllerClipBlendSystem(Animator animator, RuntimeAnimatorController controller)
    {
        playGraph = PlayableGraph.Create("ControllerClipBlend");
        AnimationPlayableOutput animOutput = AnimationPlayableOutput.Create(playGraph, "Animation", animator);

        topMixer = AnimationMixerPlayable.Create(playGraph, 2);
        animOutput.SetSourcePlayable(topMixer);

        controllerPlayable = AnimatorControllerPlayable.Create(playGraph, controller);
        topMixer.ConnectInput(0, controllerPlayable, 0);
        playGraph.GetRootPlayable(0).SetInputWeight(0, 1.0f);

        topMixer.SetInputWeight(0, 1.0f);
        topMixer.SetInputWeight(1, 0.0f);

        playGraph.Play();
    }
    public void Disable()
    {
        playGraph.Stop();
    }
    public void Enable()
    {
        playGraph.Play();
    }
    public AnimatorControllerPlayable GetController()
    {
        return controllerPlayable;
    }
    public void PlayOneShot(AnimationClip clip, float blendInTime = 0.1f, float blendOutTime = 0.1f)
    {
        if (oneShotPlayable.IsValid() && oneShotPlayable.GetAnimationClip() == clip) return;

        InterruptOneShot();
        oneShotPlayable = AnimationClipPlayable.Create(playGraph, clip);
        topMixer.ConnectInput(1, oneShotPlayable, 0);

        if(blendInTime + blendOutTime < clip.length)
        {
            float tmp = blendInTime + blendOutTime;
            blendInTime = clip.length * blendInTime / tmp;
            blendOutTime = clip.length * blendOutTime / tmp;
        }

        oneShotBlendIn = Timing.RunCoroutine(Blend(blendInTime, progress =>
        {
            float weight = progress;
            topMixer.SetInputWeight(0, 1.0f - weight);
            topMixer.SetInputWeight(1, weight);
        }));
        oneShotBlendOut = Timing.RunCoroutine(Blend(blendOutTime, progress =>
        {
            float weight = 1.0f - progress;
            topMixer.SetInputWeight(0, 1.0f - weight);
            topMixer.SetInputWeight(1, weight);
        }, clip.length - blendOutTime, DisconnectOneShot));
    }
    IEnumerator<float> Blend(float duration, Action<float> blend, float delay = 0.0f, Action onFinish = null)
    {
        if (delay > 0.0f) yield return Timing.WaitForSeconds(delay);

        if(duration > 0.0f)
        {
            float counter = 0.0f;
            while (counter / duration < 1.0f)
            {
                counter += Time.deltaTime;
                blend.Invoke(counter / duration);
                yield return counter;
            }
        }

        blend.Invoke(1.0f);
        onFinish?.Invoke();
    }
    void InterruptOneShot()
    {
        Timing.KillCoroutines(oneShotBlendIn);
        Timing.KillCoroutines(oneShotBlendOut);

        topMixer.SetInputWeight(0, 1.0f);
        topMixer.SetInputWeight(1, 0.0f);

        if (oneShotPlayable.IsValid())
        {
            DisconnectOneShot();
        }
    }
    void DisconnectOneShot()
    {
        topMixer.DisconnectInput(1);
        playGraph.DestroyPlayable(oneShotPlayable);
    }
    public void Destroy()
    {
        if (playGraph.IsValid())
        {
            InterruptOneShot();
            playGraph.Destroy();
        }
    }
}