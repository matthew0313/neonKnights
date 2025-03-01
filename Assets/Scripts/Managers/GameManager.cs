using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour, ICutsceneCallbackReceiver
{
    public static GameManager Instance { get; private set; }
    public GameManager() => Instance = this;

    public bool paused = false;
    public Action<bool> onPauseToggle;
    private void Update()
    {
        timesPlayed += Time.deltaTime;
        if (InputManager.GetPauseInput())
        {
            if (paused) Time.timeScale = 1.0f;
            else Time.timeScale = 0.0f;
            paused = !paused;
            onPauseToggle?.Invoke(paused);
        }
    }
    const int stageCount = 1;
    [SerializeField] int stageIndex = 1;
    [SerializeField] float timesPlayed = 0.0f;
    public void GameOver()
    {
        GlobalManager.Instance.LoadScene("GameOver");
    }
    public Action onStageClear;
    public void StageClear()
    {
        onStageClear?.Invoke();
    }
    public void NextStage()
    {
        if (stageIndex >= stageCount) GlobalManager.Instance.LoadScene("Ending");
        else
        {
            SaveData data = Save();
            data.stageIndex = stageIndex + 1;
            GlobalManager.Instance.LoadGame(data);
        }
    }
    public void Load(SaveData data)
    {
        timesPlayed = data.timesPlayed;
        foreach (var i in GetSavables()) i.Load(data);
    }
    public bool canSave { get; private set; } = true;
    public SaveData Save()
    {
        SaveData data = new();
        foreach(var i in GetSavables()) i.Save(data);
        data.stageIndex = stageIndex;
        data.timesPlayed = timesPlayed;
        Debug.Log("Saved");
        return data;
    }
    IEnumerable<ISavable> GetSavables()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISavable>();
    }
    private void OnDestroy()
    {
        Time.timeScale = 1.0f;
    }

    public void OnCutsceneStart()
    {
        canSave = false;
    }

    public void OnCutsceneEnd()
    {
        canSave = true;
    }
}