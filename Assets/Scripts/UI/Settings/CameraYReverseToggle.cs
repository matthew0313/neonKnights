using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraYReverseToggle : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    private void OnEnable()
    {
        Settings.onCameraYReverseChange += OnCameraYReverseChange;
        toggle.onValueChanged.AddListener(SetCameraYReverse);
        OnCameraYReverseChange();
    }
    private void OnDisable()
    {
        Settings.onCameraYReverseChange -= OnCameraYReverseChange;
        toggle.onValueChanged.RemoveListener(SetCameraYReverse);
    }
    void OnCameraYReverseChange()
    {
        toggle.isOn = Settings.cameraYReverse;
    }
    void SetCameraYReverse(bool value)
    {
        Settings.cameraYReverse = value;
    }
}