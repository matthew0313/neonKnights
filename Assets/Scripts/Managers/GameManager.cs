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

    const float autoSaveInterval = 120.0f;
    float autoSaveCounter = 0.0f;
    private void Update()
    {
        timesPlayed += Time.deltaTime;
        if (InputManager.GetPauseInput() && !isGameOver)
        {
            if (paused) Time.timeScale = 1.0f;
            else Time.timeScale = 0.0f;
            paused = !paused;
            onPauseToggle?.Invoke(paused);
        }

        autoSaveCounter += Time.deltaTime;
        if(autoSaveCounter >= autoSaveInterval)
        {
            autoSaveCounter = 0.0f;
            GlobalManager.Instance.SaveGame(0, Save());
        }
    }
    const int stageCount = 2;
    [SerializeField] int m_stageIndex = 1;
    [SerializeField] float timesPlayed = 0.0f;

    public int stageIndex => m_stageIndex;
    void Awake()
    {
        Time.timeScale = 1.0f;
        GlobalManager.Instance.onSceneWillLoad += OnSceneWillLoad;
    }
    void OnSceneWillLoad()
    {
        if(canSave && !isGameOver) GlobalManager.Instance.SaveGame(0, Save());
    }
    public void GameOver()
    {
        isGameOver = true;
        GlobalManager.Instance.LoadScene("GameOver");
    }
    public bool isGameOver { get; private set; } = false;
    public Action onStageClear;
    public void StageClear()
    {
        isGameOver = true;
        onStageClear?.Invoke();
    }
    public void NextStage()
    {
        if (stageIndex >= stageCount) GlobalManager.Instance.LoadScene("Ending");
        else
        {
            SaveData data = Save();
            data.stageIndex = stageIndex + 1;
            GlobalManager.Instance.SaveGame(0, data);
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
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ISavable>();
    }
    private void OnDestroy()
    {
        Time.timeScale = 1.0f;
        GlobalManager.Instance.onSceneWillLoad -= OnSceneWillLoad;
    }
    public int executionOrder => 99999;
    public void OnCutsceneStart()
    {
        canSave = false;
    }

    public void OnCutsceneEnd()
    {
        canSave = true;
        if(!isGameOver) GlobalManager.Instance.SaveGame(0, Save());
    }
}