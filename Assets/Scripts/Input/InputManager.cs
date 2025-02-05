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
    public InputManager() => instance = this;

    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode dashKey = KeyCode.Space;
    [SerializeField] KeyCode slotChangeLeftKey = KeyCode.LeftArrow;
    [SerializeField] KeyCode slotChangeRightKey = KeyCode.RightArrow;
    [SerializeField] KeyCode reloadKey = KeyCode.R;
    [SerializeField] KeyCode interactKey = KeyCode.E;

    [SerializeField] KeyCode inventoryTabKey = KeyCode.Tab;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    Dictionary<KeyCode, List<KeyDownAction>> keyDownActions = new();
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
    }
    public static bool GetSprintInput()
    {
        return Input.GetKey(instance.sprintKey);
    }
    public static bool GetDashInput()
    {
        return Input.GetKeyDown(instance.dashKey);
    }
    public static SlotChangeInput GetSlotChangeInput()
    {
        if (Input.GetKey(instance.slotChangeLeftKey)) return SlotChangeInput.MoveLeft;
        if (Input.GetKey(instance.slotChangeRightKey)) return SlotChangeInput.MoveRight;
        else return SlotChangeInput.Stay;
    }
    public static bool GetReloadInput()
    {
        return Input.GetKeyDown(instance.reloadKey);
    }
    public static bool GetInteractInput()
    {
        return Input.GetKeyDown(instance.interactKey);
    }
    public static bool GetInventoryTabInput()
    {
        return Input.GetKeyDown(instance.inventoryTabKey);
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