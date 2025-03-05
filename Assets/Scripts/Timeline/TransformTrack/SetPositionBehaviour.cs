using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[System.Serializable]
public class SetPositionBehaviour : PlayableBehaviour
{
    [SerializeField] Vector3 targetPos;

    [Header("Debug")]
    [SerializeField] bool retainPosition = true;

    bool firstFramePlayed = false;
    Transform target;
    Vector3 startPos;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        Transform target = playerData as Transform;
        if (target == null) return;

        this.target = target;
        if (!firstFramePlayed)
        {
            firstFramePlayed = true;
            startPos = target.position;
        }
        target.position = targetPos;
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        if (firstFramePlayed)
        {
            firstFramePlayed = false;
            if(!Application.isPlaying && retainPosition) target.position = startPos;
        }
    }
}