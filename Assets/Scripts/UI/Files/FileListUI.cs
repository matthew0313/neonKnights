using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FileListUI : MonoBehaviour
{
    [SerializeField] Transform fileListAnchor;
    [SerializeField] FileListUIElement fileListElement;

    bool initialized = false;
    private void OnEnable()
    {
        if (!initialized)
        {
            initialized = true;
            Initialize();
        }
    }
    void Initialize()
    {
        for (int i = 0; i < GlobalManager.Instance.saves.Length; i++)
        {
            FileListUIElement element = Instantiate(fileListElement, fileListAnchor);
            element.Set(i);
        }
    }
}