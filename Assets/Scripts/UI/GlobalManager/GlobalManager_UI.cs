using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalManager_UI : MonoBehaviour
{
    [SerializeField] GameObject loadingUI;
    [SerializeField] Transform loadingBar;
    public void ShowLoadingUI()
    {
        loadingUI.SetActive(true);
    }
    public void HideLoadingUI()
    {
        loadingUI.SetActive(false);
    }
    public void SetLoadingProgress(float progress)
    {
        loadingBar.localScale = new Vector2(progress, 1.0f);
    }
}