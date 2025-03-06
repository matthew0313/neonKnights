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

    const float autoSaveInterval = 60.0f;
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
            AutoSave();
        }
    }
    const int stageCount = 2;
    [SerializeField] int m_stageIndex = 1;
    [SerializeField] float timesPlayed = 0.0f;

    public int stageIndex => m_stageIndex;

    List<ISavable> savables = new();
    void Awake()
    {
        Time.timeScale = 1.0f;
        GlobalManager.Instance.onSceneWillLoad += OnSceneWillLoad;
        savables = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ISavable>().ToList();
        savables.Sort((a, b) => a.executionOrder.CompareTo(b.executionOrder));
    }
    void OnSceneWillLoad()
    {
        if(canSave && !isGameOver) GlobalManager.Instance.SaveGame(0, Save());
    }
    public void GameOver()
    {
        isGameOver = true;
        GameOverUI.saveData = Save();
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
        foreach (var i in savables) i.Load(data);
    }
    public bool canSave => !isGameOver && !inCutscene;
    public SaveData Save()
    {
        SaveData data = new();
        foreach(var i in savables) i.Save(data);
        data.stageIndex = stageIndex;
        data.timesPlayed = timesPlayed;
        Debug.Log("Saved");
        return data;
    }
    private void OnDestroy()
    {
        Time.timeScale = 1.0f;
        GlobalManager.Instance.onSceneWillLoad -= OnSceneWillLoad;
    }
    public int executionOrder => 99999;
    bool inCutscene = false;
    public void OnCutsceneStart()
    {
        inCutscene = true;
    }

    public void OnCutsceneEnd()
    {
        inCutscene = false;
        AutoSave();
    }
    void AutoSave()
    {
        if (!canSave) return;
        autoSaveCounter = 0.0f;
        GlobalManager.Instance.SaveGame(0, Save());
    }
}