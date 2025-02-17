using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCutsceneTrigger : MonoBehaviour
{
    [SerializeField] HpComp[] targets;
    [SerializeField] Cutscene triggered;
    private void Awake()
    {
        foreach (var i in targets) i.onDeath += OnTargetDeath;
    }
    void OnTargetDeath()
    {
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
            CutsceneManager.Instance.StartCutscene(triggered);
        }
    }
}