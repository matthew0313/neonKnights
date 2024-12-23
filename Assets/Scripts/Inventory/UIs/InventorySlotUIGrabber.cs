using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUIGrabber : MonoBehaviour
{
    [SerializeField] InventorySlotUI grabbingSlotUI;
    public InventoryGrabMode grabMode = InventoryGrabMode.Click;
    public Action<Item, int> onItemDrop;

    InventorySlot grabbingSlot;
    bool grabbing = false;
    private void Awake()
    {
        grabbingSlot = new InventorySlot();
        grabbingSlotUI.Set(grabbingSlot);
    }
    private void Update()
    {
        if (!grabbing)
        {
            if (grabMode == InventoryGrabMode.Click || grabMode == InventoryGrabMode.Drag)
            {
                if (InputManager.GetButtonDown())
                {
                    List<RaycastResult> results = UIScanner.ScanUI(InputManager.GetMouseInput());
                    foreach (var i in results)
                    {
                        if (i.gameObject.TryGetComponent(out InventorySlotUI slot))
                        {
                            if (slot.origin.item != null && slot.origin.Swap(grabbingSlot))
                            {
                                grabbingSlotUI.transform.position = InputManager.GetMouseInput();
                                grabbing = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            grabbingSlotUI.transform.position = InputManager.GetMouseInput();
            if (grabMode == InventoryGrabMode.Click && InputManager.GetButtonDown() || grabMode == InventoryGrabMode.Drag && !InputManager.GetButton())
            {
                List<RaycastResult> results = UIScanner.ScanUI(InputManager.GetMouseInput());
                bool slotHit = false;
                foreach (var i in results)
                {
                    if (i.gameObject.TryGetComponent(out InventorySlotUI slot))
                    {
                        slotHit = true;
                        if (grabbingSlot.item.IsStackable(slot.origin.item))
                        {
                            grabbingSlot.count = slot.origin.Insert(grabbingSlot.item, grabbingSlot.count);
                            break;
                        }
                        else if (grabbingSlot.Swap(slot.origin))
                        {
                            break;
                        }
                    }
                }
                if (!slotHit)
                {
                    grabbingSlot.item.containedSlot = null;
                    onItemDrop?.Invoke(grabbingSlot.item, grabbingSlot.count);
                }
                else if (grabbingSlot.count == 0) grabbing = false;
            }
        }
    }
    private void OnDisable()
    {
        if (grabbing)
        {
            onItemDrop?.Invoke(grabbingSlot.item, grabbingSlot.count);
            grabbingSlot.count = 0;
            grabbing = false;
        }
    }
}
public enum InventoryGrabMode
{
    Click,
    Drag
}