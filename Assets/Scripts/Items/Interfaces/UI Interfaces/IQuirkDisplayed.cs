using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public interface IQuirkDisplayed
{
    public IEnumerable<Quirk> GetQuirks();
}