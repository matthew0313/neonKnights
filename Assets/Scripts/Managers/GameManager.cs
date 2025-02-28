using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameManager() => Instance = this;

    public bool paused = false;
    public Action<bool> onPauseToggle;
    private void Update()
    {
        if (InputManager.GetPauseInput())
        {
            if (paused) Time.timeScale = 1.0f;
            else Time.timeScale = 0.0f;
            paused = !paused;
            onPauseToggle?.Invoke(paused);
        }
    }
    public void GameOver()
    {
        GlobalManager.Instance.LoadScene("GameOver");
    }
    public void StageClear()
    {
        GlobalManager.Instance.LoadScene("Title");
    }
}