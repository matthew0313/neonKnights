using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Door : MonoBehaviour, ISavable
{
    [SerializeField] Animator anim;
    [SerializeField] NavMeshLink link;
    [SerializeField] Sound openSound, closeSound;
    [SerializeField] SaveID id;
    public bool open { get; private set; } = false;

    readonly int openID = Animator.StringToHash("Open");
    public void Open()
    {
        if (open) return;
        open = true;
        anim.SetBool(openID, true);
        AudioManager.Instance.PlaySound(openSound);
    }
    public void Close()
    {
        if (!open) return;
        open = false;
        anim.SetBool(openID, false);
        AudioManager.Instance.PlaySound(closeSound);
    }
    public void Load(SaveData data)
    {
        if (data.mapElements.TryGetValue(id.value, out DataUnit tmp))
        {
            if (tmp.bools.TryGetValue("open", out bool val))
            {
                open = val;
                anim.SetBool(openID, val);
            }
        }
    }

    public void Save(SaveData data)
    {
        data.mapElements[id.value] = new();
        data.mapElements[id.value].bools["open"] = open;
    }
}