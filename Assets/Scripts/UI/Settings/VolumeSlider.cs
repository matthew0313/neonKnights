using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text valueText;
    private void OnEnable()
    {
        Settings.onVolumeChange += OnVolumeChange;
        slider.onValueChanged.AddListener(SetVolume);
        OnVolumeChange();
    }
    private void OnDisable()
    {
        Settings.onVolumeChange -= OnVolumeChange;
        slider.onValueChanged.RemoveListener(SetVolume);
    }
    void OnVolumeChange()
    {
        slider.value = Settings.volume;
        valueText.text = $"{Mathf.Round(Settings.volume * 100)}%";
    }
    void SetVolume(float value)
    {
        Settings.volume = value;
    }
}