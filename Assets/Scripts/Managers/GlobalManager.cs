using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GlobalManager_UI))]
public class GlobalManager : MonoBehaviour
{
    GlobalManager_UI UIs;
    static GlobalManager instance;
    public static GlobalManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = Instantiate(Resources.Load<GlobalManager>("GlobalManager"));
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }
    public Action onSceneWillLoad;
    AsyncOperation sceneLoading;
    private void Awake()
    {
        UIs = GetComponent<GlobalManager_UI>();
    }
    private void Update()
    {
        if (sceneLoading != null) UIs.SetLoadingProgress(sceneLoading.progress);
    }
    public void LoadScene(string sceneName)
    {
        onSceneWillLoad?.Invoke();
        UIs.ShowLoadingUI();
        AsyncOperation tmp = SceneManager.LoadSceneAsync(sceneName);
        tmp.completed += (AsyncOperation operation) =>
        {
            UIs.HideLoadingUI();
            sceneLoading = null;
        };
        UIs.SetLoadingProgress(0.0f);
        sceneLoading = tmp;
    }
}