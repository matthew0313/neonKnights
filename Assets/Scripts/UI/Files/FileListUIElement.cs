using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class FileListUIElement : MonoBehaviour
{
    [SerializeField] TMP_Text fileName, fileInfo;
    [SerializeField] Button loadButton, saveButton;
    int fileIndex;
    public void Set(int fileIndex)
    {
        this.fileIndex = fileIndex;
        saveButton.onClick.AddListener(SaveFile);
        loadButton.onClick.AddListener(LoadFile);
        SetFileInfo();
    }
    private void OnEnable()
    {
        SetFileInfo();
    }
    public void LoadFile()
    {
        if(GlobalManager.Instance.saves[fileIndex] != null)
        {
            GlobalManager.Instance.LoadGame(GlobalManager.Instance.saves[fileIndex]);
        }
    }
    public void SaveFile()
    {
        if (GameManager.Instance == null) return;
        GlobalManager.Instance.SaveGame(fileIndex, GameManager.Instance.Save(), SetFileInfo);
    }
    void SetFileInfo()
    {
        if(fileIndex == 0)
        {
            fileName.text = new LangText()
            {
                en = $"Auto Save",
                kr = $"자동 저장"
            }.text;
        }
        else
        {
            fileName.text = new LangText()
            {
                en = $"File {fileIndex}",
                kr = $"{fileIndex}번 파일"
            }.text;
        }
        if (GlobalManager.Instance.saves[fileIndex] != null)
        {
            fileInfo.text = new LangText()
            {
                en = $"Stage {GlobalManager.Instance.saves[fileIndex].stageIndex}\n" +
                $"Times played: {GetTimeCode(GlobalManager.Instance.saves[fileIndex].timesPlayed)}",
                kr = $"스테이지 {GlobalManager.Instance.saves[fileIndex].stageIndex}\n" +
                $"플레이 시간: {GetTimeCode(GlobalManager.Instance.saves[fileIndex].timesPlayed)}"
            }.text;
            loadButton.gameObject.SetActive(true);
        }
        else
        {
            fileInfo.text = new LangText() { en = "Empty", kr = "비어있음" }.text;
            loadButton.gameObject.SetActive(false);
        }

        if(GameManager.Instance != null)
        {
            saveButton.gameObject.SetActive(true);
            saveButton.interactable = GameManager.Instance.canSave;
        }
        else
        {
            saveButton.gameObject.SetActive(false);
        }
    }
    string GetTimeCode(float time)
    {
        TimeSpan span = TimeSpan.FromSeconds(time);
        return span.ToString(@"hh\:mm\:ss");
    }
}