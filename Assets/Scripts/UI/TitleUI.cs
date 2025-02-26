using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] Dropdown language;
    private void Awake()
    {
        language.value = (int)Settings.language;
        language.onValueChanged.AddListener(SetLanguage); 
    }
    void SetLanguage(int value)
    {
        Settings.language = (Language)value;
    }
    public void StartGame()
    {
        GlobalManager.Instance.LoadScene("Stage 1");
    }
}
