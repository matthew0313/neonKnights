using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneTriggerUnlocker : MonoBehaviour
{
    [SerializeField] CutsceneTrigger unlockedTrigger;
    protected void UnlockTrigger()
    {
        unlockedTrigger.Unlock();
    }
}