using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        fileName.text = new LangText()
        {
            en = $"File {fileIndex + 1}",
            kr = $"{fileIndex + 1}번 파일"
        }.text;
        if (GlobalManager.Instance.saves[fileIndex] != null)
        {
            fileInfo.text = new LangText()
            {
                en = $"Stage {GlobalManager.Instance.saves[fileIndex].stageIndex}\n" +
                $"Times played: {Mathf.Round(GlobalManager.Instance.saves[fileIndex].timesPlayed)}",
                kr = $"스테이지 {GlobalManager.Instance.saves[fileIndex].stageIndex}\n" +
                $"플레이 시간: {Mathf.Round(GlobalManager.Instance.saves[fileIndex].timesPlayed)}"
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
}