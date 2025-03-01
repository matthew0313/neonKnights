using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalManager_UI : MonoBehaviour
{
    [SerializeField] GameObject loadingUI;
    [SerializeField] Image loadingBlack;
    [SerializeField] Transform loadingBar;
    public void LoadingBlackShow(Action onBlackFinish)
    {
        loadingBlack.DOColor(new Color(0, 0, 0, 1), 0.25f).OnComplete(() =>
        {
            onBlackFinish?.Invoke();
        }).SetUpdate(true);
    }
    public void LoadingBlackHide()
    {
        loadingBlack.DOColor(new Color(0, 0, 0, 0), 0.25f).SetUpdate(true);
    }
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