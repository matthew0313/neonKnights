using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class BilingualText_TextArea : MonoBehaviour
{
    [SerializeField]
    [TextArea] string korean, english;
    TMP_Text origin;
    private void OnEnable()
    {
        if(origin == null) origin = GetComponent<TMP_Text>();
        Settings.onLanguageChange += UpdateText;
        UpdateText();
    }
    private void OnDisable()
    {
        Settings.onLanguageChange -= UpdateText;
    }
    void UpdateText()
    {
        origin.text = Settings.language == Language.English ? english : korean;
    }
}
