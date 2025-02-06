using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Melee : Weapon
{
    [SerializeField] TriggerCallbacks m_hitbox;
    public TriggerCallbacks hitbox => m_hitbox;
}