using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeathCutsceneTriggerUnlocker : CutsceneTriggerUnlocker
{
    [SerializeField] HpComp[] targets;
    private void Awake()
    {
        foreach (var i in targets) i.onDeath += OnTargetDeath;
    }
    void OnTargetDeath()
    {
        bool allDead = true;
        foreach (var i in targets)
        {
            if (i.dead == false)
            {
                allDead = false;
                break;
            }
        }
        if (allDead)
        {
            UnlockTrigger();
        }
    }
}