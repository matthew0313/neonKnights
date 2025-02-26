using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Unity.Cinemachine;
using UnityEngine.Events;
using DG.Tweening;

public class StageClearCutscene : Cutscene
{
    public override void End()
    {
        base.End();
        GameManager.Instance.StageClear();
    }
}