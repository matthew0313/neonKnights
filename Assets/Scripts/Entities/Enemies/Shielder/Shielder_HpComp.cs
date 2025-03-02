using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder_HpComp : Enemy_HpComp
{
    protected override void OnDeath()
    {
        base.OnDeath();
        gameObject.SetActive(false);
    }
}