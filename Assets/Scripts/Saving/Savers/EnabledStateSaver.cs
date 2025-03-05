using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnabledStateSaver : MonoBehaviour, ISavable
{
    [SerializeField] MonoBehaviour saved;
    [SerializeField] SaveID id;

    public void Load(SaveData data)
    {
        if(data.mapElements.TryGetValue(id.value, out DataUnit tmp))
        {
            if (tmp.bools.TryGetValue("enabled", out bool val))
            {
                saved.enabled = val;
            }
        }
    }

    public void Save(SaveData data)
    {
        data.mapElements[id.value] = new();
        data.mapElements[id.value].bools["enabled"] = saved.enabled;
    }
}