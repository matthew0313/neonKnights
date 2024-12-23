using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public interface IBarDisplayed
{
    public float barFill { get; }
    public Color barColor { get; }
}