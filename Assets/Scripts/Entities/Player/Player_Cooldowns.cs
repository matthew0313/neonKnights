using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player_Cooldowns : MonoBehaviour
{
    List<CooldownSource> cooldowns = new(), removeQueue = new(), addQueue = new();
    public void OnUpdate()
    {
        foreach(var i in cooldowns)
        {
            i.cooldownLeft = Mathf.Max(0.0f, i.cooldownLeft -= Time.deltaTime);
            if (!i.isOnCooldown)
            {
                removeQueue.Add(i);
            }
        }
        if(addQueue.Count > 0)
        {
            foreach (var i in addQueue) cooldowns.Add(i);
            addQueue.Clear();
        }
        if(removeQueue.Count > 0)
        {
            foreach (var i in removeQueue) cooldowns.Remove(i);
            removeQueue.Clear();
        }
    }
    public void AddCooldown(CooldownSource cooldown)
    {
        addQueue.Add(cooldown);
    }
    public bool ContainsCooldown(CooldownSource cooldown) => cooldowns.Contains(cooldown);
}