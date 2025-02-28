using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MEC;
using TMPro.EditorUtilities;

[RequireComponent(typeof(GlobalManager_UI))]
public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance { get; private set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        Instance = Instantiate(Resources.Load<GlobalManager>("GlobalManager"));
        DontDestroyOnLoad(Instance);
    }

    GlobalManager_UI UIs;
    public Action onSceneWillLoad;
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
    }
    public void LoadScene(string sceneName)
    {
        onSceneWillLoad?.Invoke();
        Timing.RunCoroutine(LoadingScene(sceneName));
    }
    IEnumerator<float> LoadingScene(string sceneName)
    {
        UIs.ShowLoadingUI();
        AsyncOperation tmp = SceneManager.LoadSceneAsync(sceneName);
        UIs.SetLoadingProgress(0.0f);
        while (!tmp.isDone)
        {
            UIs.SetLoadingProgress(tmp.progress);
            yield return Timing.WaitForOneFrame;
        }
        UIs.HideLoadingUI();
    }

    private void OnApplicationQuit()
    {
        SaveManager.Instance.Save("Settings.json", JsonUtility.ToJson(Settings.Save()));
    }
}