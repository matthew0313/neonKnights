using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using MEC;
using UnityEngine.InputSystem.LowLevel;

public class Player_Camera : MonoBehaviour, ICutsceneCallbackReceiver, ISavable
{
    [SerializeField] new CinemachineCamera camera;

    [SerializeField] Transform camAnchor, springAnchor;
    [SerializeField] float rotateSpeed = 1.0f, zoomSpeed = 1.0f;
    [SerializeField] float minSpringLength = 1.0f, maxSpringLength = 10.0f;
    [SerializeField] float minRotX, maxRotX;
    [SerializeField] LayerMask blockingMask;

    [SerializeField] float recoilMultiplier = 1.0f;
    [SerializeField] float recoilReductionSpeed = 5.0f;
    [SerializeField] float zoomLength = 1.0f;
    [SerializeField] float zoomTime = 0.25f;

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
                rotX = Mathf.Clamp(rotX + Input.GetAxis("Mouse Y") * -1 * rotateSpeed * Settings.cameraSensitivity * (Settings.cameraYReverse ? -1.0f : 1.0f), minRotX, maxRotX);
                rotY = rotY + Input.GetAxis("Mouse X") * rotateSpeed * Settings.cameraSensitivity;
            }
            if(UIScanner.ScanUI().Count <= 0) SetSpringLength(springLength + Input.mouseScrollDelta.y * -1 * zoomSpeed * Settings.cameraSensitivity);

            UpdateRotation();
            UpdateSpring();
        }
        recoil = Mathf.Max(0.0f, recoil - recoilReductionSpeed * Time.deltaTime);
        if (zooming) zoomCounter = Mathf.Min(zoomCounter + Time.deltaTime, zoomTime);
        else zoomCounter = Mathf.Max(zoomCounter - Time.deltaTime, 0.0f);
    }
    public bool zooming = false;
    float zoomCounter = 0.0f;

    public float zoomAmount => zoomCounter / zoomTime;
    void UpdateSpring()
    {
        float springLength = this.springLength;

        if (Physics.Raycast(springAnchor.transform.position, springAnchor.transform.forward, out RaycastHit hit, springLength, blockingMask))
        {
            camera.transform.position = hit.point;
        }
        else camera.transform.position = springAnchor.transform.position + springAnchor.transform.forward * springLength;
        camera.transform.position += camera.transform.forward * zoomLength * (zoomCounter / zoomTime);
    }
    float recoil = 0.0f;
    void UpdateRotation()
    {
        camAnchor.transform.rotation = Quaternion.Euler(rotX + recoil, rotY, 0.0f);
    }
    public void SetRot(float rotX, float rotY)
    {
        camAnchor.transform.rotation = Quaternion.Euler(rotX, rotY, 0.0f);
        this.rotX = rotX;
        this.rotY = rotY;
    }
    public void ApplyRecoil(float force)
    {
        rotX = Mathf.Clamp(rotX + recoil, minRotX, maxRotX);
        recoil = force;
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

    public void Save(SaveData data)
    {
        data.playerData.floats["cameraRotX"] = rotX;
        data.playerData.floats["cameraRotY"] = rotY;
        data.playerData.floats["cameraSpringLength"] = springLength;
    }

    public void Load(SaveData data)
    {
        float rotX, rotY, springLength;
        data.playerData.floats.TryGetValue("cameraRotX", out rotX);
        data.playerData.floats.TryGetValue("cameraRotY", out rotY);
        data.playerData.floats.TryGetValue("cameraSpringLength", out springLength);
        this.rotX = rotX;
        this.rotY = rotY;
        UpdateRotation();
        SetSpringLength(springLength);
    }
}
