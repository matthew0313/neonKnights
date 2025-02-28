using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class Player_Camera : MonoBehaviour, ICutsceneCallbackReceiver
{
    [SerializeField] new CinemachineCamera camera;

    [SerializeField] Transform camAnchor, springAnchor;
    [SerializeField] float rotateSpeed = 1.0f, zoomSpeed = 1.0f;
    [SerializeField] float minSpringLength = 1.0f, maxSpringLength = 10.0f;
    [SerializeField] float minRotX, maxRotX;
    [SerializeField] bool YReverse = true;
    [SerializeField] LayerMask blockingMask;

    [SerializeField] bool controlled = true;

    float springLength = 3.0f;
    float rotX, rotY;
    public void OnAwake()
    {
        rotX = camAnchor.transform.eulerAngles.x;
        rotY = camAnchor.transform.eulerAngles.y;
    }
    public void OnUpdate()
    {
        if (controlled && !GameManager.Instance.paused)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                SetRot(Mathf.Clamp(rotX + Input.GetAxis("Mouse Y") * -1 * rotateSpeed * Settings.cameraSensitivity * (YReverse ? -1.0f : 1.0f), minRotX, maxRotX), rotY + Input.GetAxis("Mouse X") * rotateSpeed * Settings.cameraSensitivity);
            }
            if(UIScanner.ScanUI().Count <= 0) SetSpringLength(springLength + Input.mouseScrollDelta.y * -1 * zoomSpeed * Settings.cameraSensitivity);

            UpdateSpring();
        }
    }
    void UpdateSpring()
    {
        if (Physics.Raycast(springAnchor.transform.position, springAnchor.transform.forward, out RaycastHit hit, springLength, blockingMask))
        {
            camera.transform.position = hit.point;
        }
        else camera.transform.position = springAnchor.transform.position + springAnchor.transform.forward * springLength;
    }
    public void SetRot(float rotX, float rotY)
    {
        camAnchor.transform.rotation = Quaternion.Euler(rotX, rotY, 0.0f);
        this.rotX = rotX;
        this.rotY = rotY;
    }
    public void SetSpringLength(float length)
    {
        springLength = Mathf.Clamp(length, minSpringLength, maxSpringLength);
    }

    public void OnCutsceneStart()
    {
        controlled = false;
    }

    public void OnCutsceneEnd()
    {
        controlled = true;
    }
}
