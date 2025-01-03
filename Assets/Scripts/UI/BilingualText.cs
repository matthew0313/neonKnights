using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class BilingualText : MonoBehaviour
{
    [SerializeField] LangText content;
    TMP_Text origin;
    private void OnEnable()
    {
        if(origin == null) origin = GetComponent<TMP_Text>();
        LangText.onLanguageChange += UpdateText;
        UpdateText();
    }
    private void OnDisable()
    {
        LangText.onLanguageChange -= UpdateText;
    }
    void UpdateText()
    {
        origin.text = content.text;
    }
}
