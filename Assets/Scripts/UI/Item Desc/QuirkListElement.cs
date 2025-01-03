using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuirkListElement : MonoBehaviour
{
    [SerializeField] Image backImage, quirkIcon;
    [SerializeField] TMP_Text primaryText, secondaryText;
    readonly int startColorID = Shader.PropertyToID("_Start_Color");
    readonly int endColorID = Shader.PropertyToID("_End_Color");
    public void Set(Quirk quirk)
    {
        backImage.materialForRendering.SetColor(startColorID, new Color(quirk.data.quirkColor.r, quirk.data.quirkColor.g, quirk.data.quirkColor.b, 1.0f));
        backImage.materialForRendering.SetColor(endColorID, new Color(quirk.data.quirkColor.r, quirk.data.quirkColor.g, quirk.data.quirkColor.b, 0.0f));
        quirkIcon.sprite = quirk.data.quirkIcon;
        primaryText.text = $"[{quirk.quirkName.text}] {quirk.Describe().text}";
        secondaryText.text = quirk.quirkDescription.text;
    }
}
