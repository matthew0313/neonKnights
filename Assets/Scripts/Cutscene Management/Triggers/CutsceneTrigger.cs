using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CutsceneTrigger : MonoBehaviour, ISavable
{
    [SerializeField] Cutscene cutscene;
    [SerializeField] bool locked = false;

    [Header("Saving")]
    [SerializeField] bool saveTriggerState = true;
    [SerializeField] SaveID id;
    protected bool triggered { get; private set; } = false;

    public void Unlock() => locked = false;
    protected void TriggerCutscene()
    {
        if (triggered || locked) return;
        triggered = true;
        CutsceneManager.Instance.StartCutscene(cutscene);
    }
    public void Load(SaveData data)
    {
        if (!saveTriggerState) return;
        data.cutsceneTriggers[id.value] = new();
        data.cutsceneTriggers[id.value].bools["triggered"] = triggered;
        data.cutsceneTriggers[id.value].bools["locked"] = locked;
    }

    public void Save(SaveData data)
    {
        if (!saveTriggerState) return;
        if (data.cutsceneTriggers.TryGetValue(id.value, out DataUnit tmp))
        {
            if (tmp.bools.TryGetValue("triggered", out bool tmp1)) triggered = tmp1;
            if (tmp.bools.TryGetValue("locked", out bool tmp2)) locked = tmp2;
        }
    }
}