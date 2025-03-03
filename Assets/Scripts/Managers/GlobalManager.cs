using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GlobalManager_UI))]
public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance { get; private set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        Instance = Instantiate(Resources.Load<GlobalManager>("GlobalManager"));
        DontDestroyOnLoad(Instance);
        Time.timeScale = 1.0f;
    }

    GlobalManager_UI UIs;
    public Action onSceneWillLoad;

    public SaveData[] saves { get; private set; }
    public const int saveSlotCount = 10;
    private void Awake()
    {
        UIs = GetComponent<GlobalManager_UI>();
        SaveManager.Instance.Load("Settings.json", (data) =>
        {
            if (data != null)
            {
                Settings.Load(JsonUtility.FromJson<SettingsSave>(data));
            }
        });
        saves = new SaveData[saveSlotCount + 1];
        SaveManager.Instance.Load($"AutoSave.json", (data) =>
        {
            if (data != null)
            {
                saves[0] = JsonUtility.FromJson<SaveData>(data);
            }
        });
        for (int i = 1; i <= saveSlotCount; i++)
        {
            SaveManager.Instance.Load($"Save{i}.json", (data) =>
            {
                if (data != null)
                {
                    saves[i] = JsonUtility.FromJson<SaveData>(data);
                }
            });
        }
    }
    bool sceneLoading = false;
    public void LoadScene(string sceneName, Action onLoad = null)
    {
        if (sceneLoading) return;
        sceneLoading = true;
        UIs.LoadingBlackShow(() =>
        {
            onSceneWillLoad?.Invoke();
            StartCoroutine(LoadingScene(sceneName, onLoad));
        });
    }
    IEnumerator LoadingScene(string sceneName, Action onLoad)
    {
        UIs.ShowLoadingUI();
        AsyncOperation tmp = SceneManager.LoadSceneAsync(sceneName);
        tmp.completed += (op) => onLoad?.Invoke();
        UIs.SetLoadingProgress(0.0f);
        while (!tmp.isDone)
        {
            yield return null;
            UIs.SetLoadingProgress(tmp.progress);
        }
        UIs.HideLoadingUI();
        UIs.LoadingBlackHide();
        sceneLoading = false;
    }
    public void LoadGame(SaveData data)
    {
        LoadScene($"Stage {data.stageIndex}", () =>
        {
            GameManager.Instance.Load(data);
        });
    }
    public void SaveGame(int slot, SaveData data, Action onSave = null)
    {
        if (slot < 0 || slot >= saveSlotCount) return;

        saves[slot] = data;
        if (slot == 0)
        {
            SaveManager.Instance.Save($"AutoSave.json", JsonUtility.ToJson(data, true), onSave);
        }
        else SaveManager.Instance.Save($"Save{slot}.json", JsonUtility.ToJson(data, true), onSave);
    }

    private void OnApplicationQuit()
    {
        SaveManager.Instance.Save("Settings.json", JsonUtility.ToJson(Settings.Save(), true));
    }
}