using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICutsceneCallbackReceiver
{
    public void OnCutsceneStart();
    public void OnCutsceneEnd();
}