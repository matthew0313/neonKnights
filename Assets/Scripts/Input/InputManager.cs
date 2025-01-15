using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameObject().AddComponent<InputManager>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    /*Dictionary<KeyCode, List<KeyDownAction>> keyDownActions = new();
    Dictionary<KeyDownAction, KeyCode> locator = new();
    public void AddKeyDownAction(KeyCode key, KeyDownAction action)
    {
        if (!keyDownActions.ContainsKey(key))
        {
            keyDownActions.Add(key, new());
        }
        keyDownActions[key].Add(action);
        keyDownActions[key].Sort((a, b) => b.priority.CompareTo(a.priority));
        locator.Add(action, key);
    }
    private void Update()
    {
        foreach(var i in keyDownActions.Keys)
        {
            if (Input.GetKeyDown(i))
            {
                for (int k = 0; k < keyDownActions[i].Count; k++)
                {
                    if (keyDownActions[i][k].enabled == false) continue;
                    keyDownActions[i][k].action.Invoke();
                    break;
                }
            }
        }
    }*/
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
    public static bool GetCameraRotateInput()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return Input.GetMouseButtonDown(0);
        return false;
    }
}
public enum SlotChangeInput
{
    MoveLeft = -1,
    Stay = 0,
    MoveRight = 1
}
public class KeyDownAction
{
    public readonly int priority;
    public readonly Action action;
    public bool enabled = true;
    public KeyDownAction(Action action, int priority)
    {
        this.action = action;
        this.priority = priority;
    }
}