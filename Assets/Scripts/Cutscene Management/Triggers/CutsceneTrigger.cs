using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CutsceneTrigger : MonoBehaviour, ISavable
{
    [SerializeField] Cutscene cutscene;

    [Header("Saving")]
    [SerializeField] bool saveTriggerState = true;
    [SerializeField] SaveID id;
    protected bool triggered { get; private set; } = false;
    protected void TriggerCutscene()
    {
        if (triggered || !enabled) return;
        Debug.Log("CutsceneTrigger");
        triggered = true;
        CutsceneManager.Instance.StartCutscene(cutscene);
    }
    public void Load(SaveData data)
    {
        Debug.Log(name);
        if (data.cutsceneTriggers.TryGetValue(id.value, out DataUnit tmp))
        {
            if (saveTriggerState && tmp.bools.TryGetValue("triggered", out bool tmp1)) triggered = tmp1;
            if (tmp.bools.TryGetValue("enabled", out bool tmp2)) enabled = tmp2;
        }
    }

    public void Save(SaveData data)
    {
        data.cutsceneTriggers[id.value] = new();
        if(saveTriggerState) data.cutsceneTriggers[id.value].bools["triggered"] = triggered;
        data.cutsceneTriggers[id.value].bools["enabled"] = enabled;
    }
}