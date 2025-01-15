using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
    public string interactionName;
    private bool isAdded;
    private Player_Interaction player;
    private void Awake()
    {
        player = FindObjectOfType<Player_Interaction>();
        if (player == null) return;
    }

    private void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        
        if(distance <=3f && !isAdded)
        {
            player.AddInteraction(this);
            isAdded = true;
        }
        else if(distance > 3f && isAdded)
        {
            player.RemoveInteraction(this);
            isAdded = false;
        }
    }


    public abstract void OnInteract();
}