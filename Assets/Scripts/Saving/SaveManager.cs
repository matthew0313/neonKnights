using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    static SaveManager instance;
    public static SaveManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameObject().AddComponent<SaveManager>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }

    const string savePath = "Saves";
    FileHandler handler = new LocalFileHandler();
    public void Load(string path, Action<string> onLoad)
    {
        handler.Load(Path.Combine(savePath, path), onLoad);
    }
    public void Save(string path, string content, Action onSave = null)
    {
        handler.Save(Path.Combine(savePath, path), content, onSave);
    }
}