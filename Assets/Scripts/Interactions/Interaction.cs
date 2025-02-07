using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
    public abstract LangText interactionName { get; }

    bool isAdded;
    protected Player player { get; private set; }
    private void OnEnable()
    {
        if(player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    private void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        
        if(distance <= player.interaction.interactionRange && !isAdded)
        {
            AddInteraction();
        }
        else if(distance > player.interaction.interactionRange && isAdded)
        {
            RemoveInteraction();
        }
    }
    protected void AddInteraction()
    {
        if (isAdded) return;
        isAdded = true;
        player.interaction.AddInteraction(this);
    }
    protected void RemoveInteraction()
    {
        if (!isAdded) return;
        isAdded = false;
        player.interaction.RemoveInteraction(this);
    }
    public abstract void OnInteract();
}