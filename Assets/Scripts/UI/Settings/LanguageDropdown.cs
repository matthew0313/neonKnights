using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageDropdown : MonoBehaviour
{
    [SerializeField] Dropdown dropdown;
    private void OnEnable()
    {
        Settings.onLanguageChange += OnLanguageChange;
        dropdown.onValueChanged.AddListener(SetLanguage);
        OnLanguageChange();
    }
    private void OnDisable()
    {
        Settings.onLanguageChange -= OnLanguageChange;
        dropdown.onValueChanged.RemoveListener(SetLanguage);
    }
    void OnLanguageChange()
    {
        dropdown.value = (int)Settings.language;
    }
    void SetLanguage(int value)
    {
        Settings.language = (Language)value;
    }
}