using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderCutsceneTrigger : CutsceneTrigger
{
    GameObject player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject == player)
        {
            TriggerCutscene();
        }
    }
}