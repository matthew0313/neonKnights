using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[System.Serializable]
public class SetRotationBehaviour : PlayableBehaviour
{
    [SerializeField] Vector3 targetEulers;

    [Header("Debug")]
    [SerializeField] bool retainPosition = true;

    bool firstFramePlayed = false;
    Transform target;
    Vector3 startEulers;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        Transform target = playerData as Transform;
        if (target == null) return;

        this.target = target;
        if (!firstFramePlayed)
        {
            firstFramePlayed = true;
            startEulers = target.eulerAngles;
        }
        target.eulerAngles = targetEulers;
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        if (firstFramePlayed)
        {
            firstFramePlayed = false;
            if(!Application.isPlaying && retainPosition) target.eulerAngles = startEulers;
        }
    }
}