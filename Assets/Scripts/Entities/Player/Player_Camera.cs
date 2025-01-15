using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    [SerializeField] SpringArm camAnchor;
    [SerializeField] Camera cam;
    public bool canRotateCamera = true;
    public void OnUpdate()
    {
        if (!canRotateCamera) return;
        if (InputManager.GetCameraRotateInput())
        {

        }
    }
}
