using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy_HpComp : HpComp
{
    protected override void OnDeath()
    {
        base.OnDeath();
        Revive();
    }
}
