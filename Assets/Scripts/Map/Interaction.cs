using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
    [SerializeField] string m_interactionName;
    public string interactionName => m_interactionName;

    bool isAdded;
    Player_Interaction player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Interaction>();
    }
    private void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        
        if(distance <= player.interactionRange && !isAdded)
        {
            AddInteraction();
        }
        else if(distance > player.interactionRange && isAdded)
        {
            RemoveInteraction();
        }
    }
    protected void AddInteraction()
    {
        if (isAdded) return;
        isAdded = true;
        player.AddInteraction(this);
    }
    protected void RemoveInteraction()
    {
        if (!isAdded) return;
        isAdded = false;
        player.RemoveInteraction(this);
    }
    public abstract void OnInteract();
}