using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class Player_Camera : MonoBehaviour, ICutsceneCallbackReceiver
{
    [SerializeField] new CinemachineCamera camera;

    [SerializeField] Transform camAnchor;
    [SerializeField] float rotateSpeed = 1.0f, zoomSpeed = 1.0f;
    [SerializeField] float minCameraDist = 1.0f, maxCameraDist = 10.0f;
    [SerializeField] float minRotX, maxRotX;
    [SerializeField] bool YReverse = true;
    [SerializeField] LayerMask blockingMask;

    [SerializeField] bool controlled = true;

    float armLength = 3.0f;
    float rotX, rotY;
    public void OnAwake()
    {
        rotX = camAnchor.transform.eulerAngles.x;
        rotY = camAnchor.transform.eulerAngles.y;
    }
    public void OnUpdate()
    {
        if (controlled)
        {
            if (Input.GetMouseButton(1))
            {
                SetRot(Mathf.Clamp(rotX + Input.GetAxis("Mouse Y") * -1 * rotateSpeed * Settings.cameraSensitivity * (YReverse ? -1.0f : 1.0f), minRotX, maxRotX), rotY + Input.GetAxis("Mouse X") * rotateSpeed * Settings.cameraSensitivity);
            }
            if(UIScanner.ScanUI().Count <= 0) SetZoom(armLength + Input.mouseScrollDelta.y * -1 * zoomSpeed * Settings.cameraSensitivity);

            if (Physics.Raycast(camAnchor.transform.position, camAnchor.transform.forward, out RaycastHit hit, armLength, blockingMask))
            {
                camera.transform.position = hit.point;
            }
            else camera.transform.position = camAnchor.transform.position + camAnchor.transform.forward * armLength;
        }
    }
    public void SetRot(float rotX, float rotY)
    {
        camAnchor.transform.rotation = Quaternion.Euler(rotX, rotY, 0.0f);
        this.rotX = rotX;
        this.rotY = rotY;
    }
    public void SetZoom(float zoom)
    {
        armLength = Mathf.Clamp(zoom, minCameraDist, maxCameraDist);
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
