using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Text;
using UnityEditor.UIElements;
using Unity.VisualScripting;

public class InventorySlotUIHandler : MonoBehaviour
{
    [SerializeField] InventorySlotUI grabbingSlotUI;
    public InventoryGrabMode grabMode = InventoryGrabMode.Click;
    Player_Inventory player;

    [Header("Description")]
    [SerializeField] RectTransform describer;
    [SerializeField] TMP_Text itemName;
    [SerializeField] GameObject itemCategory;
    [SerializeField] TMP_Text itemCategoryText, itemDesc;

    [Header("StatDisplay")]
    [SerializeField] Transform itemStatAnchor;
    [SerializeField] TMP_Text itemStatPrefab;
    List<TMP_Text> itemStatList = new();

    [Header("QuirkDisplay")]
    [SerializeField] Transform itemQuirkAnchor;
    [SerializeField] QuirkListElement itemQuirkPrefab;
    List<QuirkListElement> itemQuirkList = new();

    [Header("QuirkShardDisplay")]
    [SerializeField] GameObject quirkShardDisplay;
    [SerializeField] Image quirkShardBackImage;
    [SerializeField] TMP_Text quirkShardQuirkDesc;
    readonly int startColorID = Shader.PropertyToID("_Start_Color");
    readonly int endColorID = Shader.PropertyToID("_End_Color");


    InventorySlot grabbingSlot;
    bool grabbing => grabbingSlot.item != null;
    private void Awake()
    {
        grabbingSlot = new InventorySlot();
        grabbingSlotUI.Set(grabbingSlot);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Inventory>();
        quirkShardBackImage.material = new Material(quirkShardBackImage.material);
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
            if (Input.GetMouseButtonDown(0))
            {
                List<RaycastResult> results = UIScanner.ScanUI();
                foreach (var i in results)
                {
                    if (i.gameObject.TryGetComponent(out InventorySlotUI slot))
                    {
                        if (slot.origin.item != null && slot.origin.Swap(grabbingSlot))
                        {
                            grabbingSlotUI.transform.position = Input.mousePosition;
                            break;
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                List<RaycastResult> results = UIScanner.ScanUI();
                foreach (var i in results)
                {
                    if (i.gameObject.TryGetComponent(out InventorySlotUI slot))
                    {
                        if (slot.origin.item != null && grabbingSlot.CanSetItem(slot.origin.item))
                        {
                            int count = slot.origin.count / 2 + slot.origin.count % 2;
                            grabbingSlot.SetItem(slot.origin.item.Copy());
                            grabbingSlot.count = count;
                            slot.origin.count -= count;
                            grabbingSlotUI.transform.position = Input.mousePosition;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            grabbingSlotUI.transform.position = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                List<RaycastResult> results = UIScanner.ScanUI();
                bool slotHit = false;
                foreach (var i in results)
                {
                    if (i.gameObject.TryGetComponent(out InventorySlotUI slot))
                    {
                        if(grabbingSlot.item is IApplied && slot.origin.item != null)
                        {
                            Debug.Log(grabbingSlot.count);
                            (grabbingSlot.item as IApplied).ApplyTo(slot.origin.item);
                            return;
                        }
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
                if (!slotHit && grabbingSlot.item.canDrop)
                {
                    //DropItem(grabbingSlot.item, grabbingSlot.count);
                    //grabbingSlot.SetItem(null);
                }
            }
            else if(grabMode == InventoryGrabMode.Click && Input.GetMouseButtonDown(1))
            {
                List<RaycastResult> results = UIScanner.ScanUI();
                bool slotHit = false;
                foreach (var i in results)
                {
                    if (i.gameObject.TryGetComponent(out InventorySlotUI slot))
                    {
                        slotHit = true;
                        grabbingSlot.count -= 1 - slot.origin.Insert(grabbingSlot.item.Copy(), 1);
                        break;
                    }
                }
                if (!slotHit && grabbingSlot.item.canDrop)
                {
                    //DropItem(grabbingSlot.item.Copy(), 1);
                    //grabbingSlot.count--;
                }
            }
        }
    }
    Item describing;
    void DescUpdate()
    {
        if (grabbingSlot.item == null)
        {
            List<RaycastResult> results = UIScanner.ScanUI();
            bool detected = false;
            foreach (var i in results)
            {
                if (i.gameObject.TryGetComponent(out InventorySlotUI slot) && slot.origin.item != null)
                {
                    detected = true;
                    if (slot.origin.item != describing) Describe(slot.origin.item);
                    describer.pivot = new Vector2((Input.mousePosition.x < Screen.width / 2.0f ? 0.0f : 1.0f), (Input.mousePosition.y < Screen.height / 2.0f ? 0.0f : 1.0f));
                    describer.position = Input.mousePosition;
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
        itemName.text = item.GetName().text;
        if (item.data.category != 0)
        {
            StringBuilder sb = new();
            bool first = true;
            foreach(ItemCategory i in Enum.GetValues(typeof(ItemCategory)))
            {
                if (item.data.category.HasFlag(i))
                {
                    if (!first) sb.Append(", ");
                    first = false;
                    sb.Append(Item.CategoryToLangText(i).text);
                }
            }
            itemCategoryText.text = sb.ToString();
            itemCategory.gameObject.SetActive(true);
        }
        else itemCategory.gameObject.SetActive(false);
        if (item.GetDescription().text != string.Empty)
        {
            itemDesc.text = item.GetDescription().text;
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
                itemStatList[index++].text = i.text;
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
            int tot = 0;
            foreach (var i in (item as IQuirkDisplayed).GetQuirks())
            {
                if (itemQuirkList.Count <= index) itemQuirkList.Add(Instantiate(itemQuirkPrefab, itemQuirkAnchor));
                itemQuirkList[index].gameObject.SetActive(true);
                itemQuirkList[index++].Set(i);
                tot++;
            }
            for (; index < itemQuirkList.Count; index++)
            {
                itemQuirkList[index].gameObject.SetActive(false);
            }
            itemQuirkAnchor.gameObject.SetActive(tot > 0);
        }
        else itemQuirkAnchor.gameObject.SetActive(false);
        if(item is QuirkShard_Item)
        {
            QuirkShard_Item tmp = item as QuirkShard_Item;
            Color tmp2 = tmp.quirkShardOf.quirkColor;
            quirkShardDisplay.SetActive(true);
            quirkShardBackImage.material.SetColor(startColorID, new Color(tmp2.r, tmp2.g, tmp2.b, 1.0f));
            quirkShardBackImage.material.SetColor(endColorID, new Color(tmp2.r, tmp2.g, tmp2.b, 0.0f));
            quirkShardQuirkDesc.text = $"[{tmp.quirkShardOf.quirkName.text}]: {tmp.quirkShardOf.quirkDescription.text}";
        }
        else quirkShardDisplay.SetActive(false);
    }
    void DropItem(Item item, int count)
    {
        player.DropItem(item, count);
    }
    private void OnDisable()
    {
        if (grabbing)
        {
            DropItem(grabbingSlot.item, grabbingSlot.count);
            grabbingSlot.count = 0;
        }
    }
}
public enum InventoryGrabMode
{
    Click,
    Drag
}