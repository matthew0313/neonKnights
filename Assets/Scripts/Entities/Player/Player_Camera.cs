using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    [SerializeField] SpringArm camAnchor;
    [SerializeField] float rotateSpeed = 1.0f, zoomSpeed = 1.0f;
    [SerializeField] float minCameraDist = 1.0f, maxCameraDist = 10.0f;
    [SerializeField] float minRotX, maxRotX;
    public bool canRotateCamera = true, canZoomCamera = true;

    float rotX, rotY;
    public void OnAwake()
    {
        rotX = camAnchor.transform.eulerAngles.x;
        rotY = camAnchor.transform.eulerAngles.y;
    }
    public void OnUpdate()
    {
        if (canRotateCamera)
        {
            if (Input.GetMouseButton(1))
            {
                SetRot(Mathf.Clamp(rotX + Input.GetAxis("Mouse Y") * -1 * rotateSpeed * Settings.cameraSensitivity, minRotX, maxRotX), rotY + Input.GetAxis("Mouse X") * rotateSpeed * Settings.cameraSensitivity);
            }
        }
        if (canZoomCamera && UIScanner.ScanUI().Count <= 0)
        {
            SetZoom(camAnchor.armLength + Input.mouseScrollDelta.y * -1 * zoomSpeed * Settings.cameraSensitivity);
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
        camAnchor.armLength = Mathf.Clamp(zoom, minCameraDist, maxCameraDist);
    }
}
