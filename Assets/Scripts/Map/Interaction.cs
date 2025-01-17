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
            player.AddInteraction(this);
            isAdded = true;
        }
        else if(distance > player.interactionRange && isAdded)
        {
            player.RemoveInteraction(this);
            isAdded = false;
        }
    }
    public abstract void OnInteract();
}