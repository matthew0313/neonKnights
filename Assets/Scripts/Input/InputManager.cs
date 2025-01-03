using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static bool GetButtonDown()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return Input.GetMouseButtonDown(0);
        return false;
    }
    public static bool GetButton()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return Input.GetMouseButton(0);
        return false;
    }
    public static Vector2 GetMovementInput()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        return Vector2.zero;
    }
    public static bool GetSprintInput()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return Input.GetKey(KeyCode.LeftShift);
        return false;
    }
    public static bool GetDashInput()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return Input.GetKeyDown(KeyCode.Space);
        return false;
    }
    public static Vector2 GetMouseInput()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return Input.mousePosition;
        return Vector2.zero;
    }
    public static SlotChangeInput GetSlotChangeInput()
    {
        if(SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetKey(KeyCode.LeftArrow)) return SlotChangeInput.MoveLeft;
            if (Input.GetKey(KeyCode.RightArrow)) return SlotChangeInput.MoveRight;
            else return SlotChangeInput.Stay;
        }
        return SlotChangeInput.Stay;
    }
    public static bool GetReloadInput()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return Input.GetKeyDown(KeyCode.R);
        return false;
    }
}
public enum SlotChangeInput
{
    MoveLeft = -1,
    Stay = 0,
    MoveRight = 1
}