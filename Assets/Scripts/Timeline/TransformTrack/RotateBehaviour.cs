using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[System.Serializable]
public class RotateBehaviour : PlayableBehaviour
{
    [SerializeField] Vector3 targetEulers;
    [SerializeField] AnimationCurve rotateCurve;
    [SerializeField] bool local = false;

    [Header("Debug")]
    [SerializeField] bool retainPosition = true;

    Transform target;
    Vector3 startEulers;
    bool firstFramePlayed = false;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        Transform target = playerData as Transform;
        if (target == null) return;

        this.target = target;
        if (!firstFramePlayed)
        {
            startEulers = local ? target.localRotation.eulerAngles : target.rotation.eulerAngles;
            firstFramePlayed = true;
        }
        if (!local)
        {
            target.rotation = Quaternion.Slerp(Quaternion.Euler(startEulers), Quaternion.Euler(targetEulers), rotateCurve.Evaluate((float)playable.GetTime() / (float)playable.GetDuration()));
        }
        else
        {
            target.localRotation = Quaternion.Slerp(Quaternion.Euler(startEulers), Quaternion.Euler(targetEulers), rotateCurve.Evaluate((float)playable.GetTime() / (float)playable.GetDuration()));
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
                if (!local) target.rotation = Quaternion.Euler(startEulers);
                else target.localRotation = Quaternion.Euler(startEulers);
            }
            else
            {
                if(!local) target.rotation = Quaternion.Euler(targetEulers);
                else target.localRotation = Quaternion.Euler(targetEulers);
            }
        }
    }
}