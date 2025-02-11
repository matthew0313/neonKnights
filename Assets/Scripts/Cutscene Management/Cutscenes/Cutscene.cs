using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Cutscene : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] PlayableDirector director;
    private void Awake()
    {
        foreach(var i in anim.GetBehaviours<TimelinePlayBehaviour>())
        {
            i.playTimeline += PlayTimeline;
        }
        foreach(var i in anim.GetBehaviours<CutsceneFinishBehaviour>())
        {
            i.finishCutscene += FinishCutscene;
        }
    }
    readonly int playID = Animator.StringToHash("Play");
    readonly int progressID = Animator.StringToHash("Progress");
    private void Update()
    {
        if(director.playableAsset != null && director.state == PlayState.Paused)
        {
            anim.SetTrigger(progressID);
        }
    }
    public virtual void Play()
    {
        enabled = true;
        anim.SetTrigger(playID);
    }
    void PlayTimeline(TimelineAsset timeline)
    {
        director.Play(timeline);
    }
    void FinishCutscene()
    {
        enabled = false;
    }
}