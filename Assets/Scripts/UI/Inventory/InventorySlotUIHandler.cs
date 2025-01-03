using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Text;
using UnityEditor.UIElements;

public class InventorySlotUIHandler : MonoBehaviour
{
    [SerializeField] InventorySlotUI grabbingSlotUI;
    public InventoryGrabMode grabMode = InventoryGrabMode.Click;
    public Action<Item, int> onItemDrop;

    [Header("Description")]
    [SerializeField] GameObject describer;
    [SerializeField] TMP_Text itemName, itemCategory, itemDesc;

    [SerializeField] Transform itemStatAnchor;
    [SerializeField] TMP_Text itemStatPrefab;
    List<TMP_Text> itemStatList = new();

    [SerializeField] Transform itemQuirkAnchor;
    [SerializeField] QuirkListElement itemQuirkPrefab;
    List<QuirkListElement> itemQuirkList = new();
    

    InventorySlot grabbingSlot;
    bool grabbing = false;
    private void Awake()
    {
        grabbingSlot = new InventorySlot();
        grabbingSlotUI.Set(grabbingSlot);
    }
    private void Update()
    {
        GrabUpdate();
        DescUpdate();
        
    }
    void GrabUpdate()
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
    Item describing;
    void DescUpdate()
    {
        if (!grabbing)
        {
            List<RaycastResult> results = UIScanner.ScanUI(InputManager.GetMouseInput());
            bool detected = false;
            foreach (var i in results)
            {
                if (i.gameObject.TryGetComponent(out InventorySlotUI slot) && slot.origin.item != null)
                {
                    detected = true;
                    describer.transform.position = InputManager.GetMouseInput();
                    if (slot.origin.item != describing) Describe(slot.origin.item);
                    describer.gameObject.SetActive(true);
                    break;
                }
            }
            if (!detected)
            {
                describing = null;
                describer.gameObject.SetActive(false);
            }
        }
        else
        {
            describing = null;
            describer.gameObject.SetActive(false);
        }
    }
    void Describe(Item item)
    {
        if(describing != null)
        {

        }
        describing = item;
        if (item.data.category != 0)
        {
            StringBuilder sb = new();
            sb.Append("Category: ");
            bool first = true;
            foreach(Enum i in Enum.GetValues(typeof(ItemCategory)))
            {
                if (item.data.category.HasFlag(i))
                {
                    if (!first) sb.Append(", ");
                    first = false;
                    sb.Append(i);
                }
            }
            itemCategory.text = sb.ToString();
            itemCategory.gameObject.SetActive(true);
        }
        else itemCategory.gameObject.SetActive(false);
        if (item.data.description.text != string.Empty)
        {
            itemDesc.text = item.data.description.text;
            itemDesc.gameObject.SetActive(true);
        }
        else itemDesc.gameObject.SetActive(false);
        if (item is IStatDisplayed)
        {
            int index = 0;
            foreach (var i in (item as IStatDisplayed).GetStats())
            {
                if (itemStatList.Count <= index) itemStatList.Add(Instantiate(itemStatPrefab, itemStatAnchor));
                itemStatList[index].gameObject.SetActive(true);
                itemStatList[index++].text = i;
            }
            for (; index < itemStatList.Count; index++)
            {
                itemStatList[index].gameObject.SetActive(false);
            }
            itemStatAnchor.gameObject.SetActive(true);
        }
        else itemStatAnchor.gameObject.SetActive(false);
        if (item is IQuirkDisplayed)
        {
            int index = 0;
            foreach (var i in (item as IQuirkDisplayed).GetQuirks())
            {
                if (itemQuirkList.Count <= index) itemQuirkList.Add(Instantiate(itemQuirkPrefab, itemQuirkAnchor));
                itemQuirkList[index].gameObject.SetActive(true);
                itemQuirkList[index++].Set(i);
            }
            for (; index < itemQuirkList.Count; index++)
            {
                itemQuirkList[index].gameObject.SetActive(false);
            }
            itemQuirkAnchor.gameObject.SetActive(true);
        }
        else itemQuirkAnchor.gameObject.SetActive(false);
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