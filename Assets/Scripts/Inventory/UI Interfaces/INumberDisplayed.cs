using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public interface INumberDisplayed
{
    public string numberText { get; }
    public Action onNumberUpdate { get; set; }
}