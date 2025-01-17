using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class RainbowText : MonoBehaviour
{
    [SerializeField] float saturation = 0.5f, value = 1.0f;
    [SerializeField] float rainbowSpeed = 1.0f;
    TMP_Text text;
    private void OnEnable()
    {
        if(text == null) text = GetComponent<TMP_Text>();
    }
    float counter = 0.0f;
    private void Update()
    {
        counter += Time.deltaTime;
        if (counter > 1.0f) counter -= 1.0f;
        Color tmp = Color.HSVToRGB(counter, saturation, value);
        text.color = new Color(tmp.r, tmp.g, tmp.b, text.color.a);
    }
}