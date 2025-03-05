using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[System.Serializable]
public class MoveToBehaviour : PlayableBehaviour
{
    [SerializeField] Vector3 targetPos;
    [SerializeField] AnimationCurve moveCurve;
    [SerializeField] bool local = false;

    [Header("Debug")]
    [SerializeField] bool retainPosition = true;

    Transform target;
    Vector3 startPos;
    bool firstFramePlayed = false;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        Transform target = playerData as Transform;
        if (target == null) return;

        this.target = target;
        if (!firstFramePlayed)
        {
            startPos = local ? target.localPosition : target.position;
            firstFramePlayed = true;
        }
        if (!local)
        {
            target.position = Vector3.Lerp(startPos, targetPos, moveCurve.Evaluate((float)playable.GetTime() / (float)playable.GetDuration()));
        }
        else
        {
            target.localPosition = Vector3.Lerp(startPos, targetPos, moveCurve.Evaluate((float)playable.GetTime() / (float)playable.GetDuration()));
        }
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        if (firstFramePlayed)
        {
            firstFramePlayed = false;
            if (!Application.isPlaying && retainPosition)
            {
                if (!local) target.position = startPos;
                else target.localPosition = startPos;
            }
            else
            {
                if (!local) target.position = targetPos;
                else target.localPosition = targetPos;
            }
        }
    }
}