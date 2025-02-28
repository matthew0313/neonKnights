using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_UI : MonoBehaviour
{
    [SerializeField] GameObject pauseUI;
    public void ShowPauseUI() => pauseUI.SetActive(true);
    public void HidePauseUI() => pauseUI.SetActive(false);
}