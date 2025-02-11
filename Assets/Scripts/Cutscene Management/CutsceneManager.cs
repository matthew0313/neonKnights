using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }
    public CutsceneManager() => Instance = this;

    Cutscene playing;
    public void StartCutscene(Cutscene cutscene)
    {
        if (playing != null) return;
        playing = cutscene;
        foreach(var i in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICutsceneCallbackReceiver>())
        {
            i.OnCutsceneStart();
        }
    }
    public void FinishCutscene()
    {
        if (playing == null) return;
        playing = null;
        foreach (var i in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICutsceneCallbackReceiver>())
        {
            i.OnCutsceneEnd();
        }
    }
}