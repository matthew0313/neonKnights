using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cooldown Source", menuName = "Scriptables/Cooldown Source", order = 1)]
public class CooldownSource : ScriptableObject
{
    public float cooldownLeft = 0.0f;
    public bool isOnCooldown => cooldownLeft > 0.0f;
}