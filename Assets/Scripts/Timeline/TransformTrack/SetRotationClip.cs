using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class SetRotationClip : PlayableAsset, ITimelineClipAsset
{
    [SerializeField] SetRotationBehaviour template;
    public ClipCaps clipCaps => ClipCaps.Blending;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<SetRotationBehaviour>.Create(graph, template);
    }
}
