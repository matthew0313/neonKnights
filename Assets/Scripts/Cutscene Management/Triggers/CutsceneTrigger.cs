using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CutsceneTrigger : MonoBehaviour, ISavable
{
    [SerializeField] Cutscene cutscene;
    [SerializeField] SaveID id;
    protected bool triggered { get; private set; } = false;
    protected void TriggerCutscene()
    {
        if (triggered) return;
        triggered = true;
        CutsceneManager.Instance.StartCutscene(cutscene);
    }
    public void Load(SaveData data)
    {
        data.cutsceneTriggers[id.value] = triggered;
    }

    public void Save(SaveData data)
    {
        if (data.cutsceneTriggers.TryGetValue(id.value, out bool tmp)) triggered = tmp;
    }
}