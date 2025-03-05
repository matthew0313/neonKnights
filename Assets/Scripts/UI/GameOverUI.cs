using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public static SaveData saveData;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void init()
    {
        saveData = null;
    }

    private void Start()
    {
        if (saveData == null) return;
    }
}
