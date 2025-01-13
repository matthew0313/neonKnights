using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public interface IStatDisplayed
{
    public IEnumerable<LangText> GetStats();
}