using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cooldown Source", menuName = "Scriptables/Cooldown Source", order = 1)]
public class CooldownSource : ScriptableObject
{
    public void SetCooldown(float cooldown)
    {
        startCooldown = cooldown;
        cooldownLeft = cooldown;
    }
    public float startCooldown { get; private set; } = 1.0f;
    public float cooldownLeft = 0.0f;
    public bool isOnCooldown => cooldownLeft > 0.1f;
}