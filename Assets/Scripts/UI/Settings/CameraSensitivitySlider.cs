using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraSensitivitySlider : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text valueText;
    private void OnEnable()
    {
        Settings.onCameraSensitivityChange += OnCameraSensitivityChange;
        slider.onValueChanged.AddListener(SetCameraSensitivity);
        OnCameraSensitivityChange();
    }
    private void OnDisable()
    {
        Settings.onCameraSensitivityChange -= OnCameraSensitivityChange;
        slider.onValueChanged.RemoveListener(SetCameraSensitivity);
    }
    void OnCameraSensitivityChange()
    {
        slider.value = Settings.cameraSensitivity;
        valueText.text = $"{Mathf.Round(Settings.cameraSensitivity * 100)}%";
    }
    void SetCameraSensitivity(float value)
    {
        Settings.cameraSensitivity = value;
    }
}