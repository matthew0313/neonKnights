using UnityEngine;
using TMPro;

public class CameraLooker : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }
}