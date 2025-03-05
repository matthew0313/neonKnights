using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeathTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent triggered;
    [SerializeField] HpComp[] targets;

    private void Awake()
    {
        foreach (var i in targets) i.onDeath += OnTargetDeath;
    }
    void OnTargetDeath()
    {
        if (!enabled) return;

        bool allDead = true;
        foreach(var i in targets)
        {
            if(i.dead == false)
            {
                allDead = false;
                break;
            }
        }
        if (allDead)
        {
            triggered.Invoke();
        }
    }
}