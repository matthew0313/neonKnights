using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public interface IReloadDisplayed
{
    public bool reloading { get; }
    public float reloadProgress { get; }
}