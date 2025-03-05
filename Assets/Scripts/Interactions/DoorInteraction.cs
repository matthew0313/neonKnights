using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class DoorInteraction : Interaction
{
    [SerializeField] Door door;
    [SerializeField] bool canClose = true;
    public override LangText interactionName => door.open ? new LangText()
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
        if (door.open && !canClose) enabled = false;
    }
    public override void OnInteract()
    {
        if (door.open)
        {
            if(canClose) door.Close();
        }
        else
        {
            door.Open();
            if (!canClose) enabled = false;
        }
    }
}