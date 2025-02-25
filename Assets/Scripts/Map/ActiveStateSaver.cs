using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveStateSaver : MonoBehaviour, ISavable
{
    [SerializeField] SaveID id;
    public void Load(SaveData data)
    {
        if(data.mapElements.TryGetValue(id.value, out DataUnit tmp))
        {
            if (tmp.bools.TryGetValue("active", out bool val)) gameObject.SetActive(val);
        }
    }

    public void Save(SaveData data)
    {
        data.mapElements[id.value] = new();
        data.mapElements[id.value].bools["active"] = gameObject.activeSelf;
    }
}