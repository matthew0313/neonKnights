using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class DoorInteraction : Interaction, ISavable
{
    [SerializeField] Animator anim;
    [SerializeField] NavMeshLink link;
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
    protected override void Start()
    {
        base.Start();
        if(link != null) link.enabled = anim.GetBool(openID);
    }

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
        if(link != null) link.enabled = !tmp;
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
            if(tmp.bools.TryGetValue("Open", out bool val))
            {
                anim.SetBool(openID, val);
                if(link != null) link.enabled = val;
            }
        }
    }
}