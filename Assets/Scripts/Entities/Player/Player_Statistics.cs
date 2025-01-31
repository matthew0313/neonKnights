using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player_Statistics : MonoBehaviour, ISpeedMultiplierAdjustable, IDamageMultiplierAdjustable, IDamageTakenMultiplierAdjustable
{
    public string playerName = "NoName";
    public float speedMultiplier { get; set; } = 1.0f;
    public float damageMultiplier { get; set; } = 1.0f;
    public float damageTakenMultiplier { get; set; } = 1.0f;
    public bool canBeDetected = true;
}