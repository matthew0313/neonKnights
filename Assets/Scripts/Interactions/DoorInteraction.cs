using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : Interaction, ISavable
{
    [SerializeField] Animator anim;
    [SerializeField] protected SaveID id;
    [SerializeField] Sound openSound, closeSound;
    readonly int openID = Animator.StringToHash("Open");
    public override LangText interactionName => anim.GetBool(openID) ? new LangText()
    {
        en = "Close Door",
        kr = "문 닫기"
    } : new LangText()
    {
        en = "Open Door",
        kr = "문 열기"
    };

    public override void OnInteract()
    {
        bool tmp = anim.GetBool(openID);
        if (tmp)
        {
            AudioManager.Instance.PlaySound(closeSound);
        }
        else
        {
            AudioManager.Instance.PlaySound(openSound);
        }
        anim.SetBool(openID, !tmp);
    }

    public virtual void Save(SaveData data)
    {
        data.mapElements[id.value] = new();
        data.mapElements[id.value].bools["Open"] = anim.GetBool(openID);
    }

    public virtual void Load(SaveData data)
    {
        if(data.mapElements.TryGetValue(id.value, out DataUnit tmp))
        {
            if(tmp.bools.TryGetValue("Open", out bool val)) anim.SetBool(openID, val);
        }
    }
}