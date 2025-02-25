using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slasher_HpComp : Enemy_HpComp
{
    protected override void OnDeath()
    {
        base.OnDeath();
        Destroy(gameObject);
    }
}