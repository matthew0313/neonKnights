using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder_HpComp : HpComp
{
    protected override void OnDeath()
    {
        base.OnDeath();
        Destroy(gameObject);
    }
}