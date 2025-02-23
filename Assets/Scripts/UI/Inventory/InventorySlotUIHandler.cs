using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Text;
public class InventorySlotUIHandler : MonoBehaviour
{
    public static InventorySlotUIHandler Instance { get; private set; }
    public InventorySlotUIHandler() => Instance = this;

    [SerializeField] InventorySlotUI grabbingSlotUI;
    Player_Inventory player;

    [Header("Description")]
    [SerializeField] RectTransform describer;
    [SerializeField] TMP_Text itemName;
    [SerializeField] GameObject itemCategory;
    [SerializeField] TMP_Text itemCategoryText, itemDesc;
    [SerializeField] GameObject undroppable;

    [Header("StatDisplay")]
    [SerializeField] Transform itemStatAnchor;
    [SerializeField] TMP_Text itemStatPrefab;
    List<TMP_Text> itemStatList = new();

    [Header("QuirkDisplay")]
    [SerializeField] Transform itemQuirkAnchor;
    [SerializeField] QuirkListElement itemQuirkPrefab;
    List<QuirkListElement> itemQuirkList = new();

    [Header("QuirkApplierDisplay")]
    [SerializeField] GameObject quirkApplierDisplay;
    [SerializeField] Image quirkApplierBackImage;
    [SerializeField] TMP_Text quirkApplierQuirkDesc;
    readonly int startColorID = Shader.PropertyToID("_Start_Color");
    readonly int endColorID = Shader.PropertyToID("_End_Color");


    public InventorySlot grabbingSlot { get; } = new();
    bool grabbing => grabbingSlot.item != null;
    private void Awake()
    {
        grabbingSlotUI.Set(grabbingSlot);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Inventory>();
        quirkApplierBackImage.material = new Material(quirkApplierBackImage.material);
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
                if (!slotHit && grabbingSlot.item.data.canDrop)
                {
                    DropItem(grabbingSlot.item.Copy(), grabbingSlot.count);
                    grabbingSlot.count = 0;
                }
            }
            else if(Input.GetMouseButtonDown(1))
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
                if (!slotHit && grabbingSlot.item.data.canDrop)
                {
                    DropItem(grabbingSlot.item.Copy(), 1);
                    grabbingSlot.count--;
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
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000.0f, LayerMask.GetMask("DroppedItem"));
                if(hit.transform != null && hit.transform.TryGetComponent(out DroppedItem droppedItem))
                {
                    if (droppedItem.slot.item != describing) Describe(droppedItem.slot.item);
                    describer.pivot = new Vector2((Input.mousePosition.x < Screen.width / 2.0f ? 0.0f : 1.0f), (Input.mousePosition.y < Screen.height / 2.0f ? 0.0f : 1.0f));
                    describer.position = Input.mousePosition;
                    describer.gameObject.SetActive(true);
                }
                else
                {
                    describing = null;
                    describer.gameObject.SetActive(false);
                }
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
        if(item is QuirkApplierItem)
        {
            QuirkApplierItem tmp = item as QuirkApplierItem;
            Color tmp2 = tmp.data.appliedQuirk.quirkColor;
            quirkApplierDisplay.SetActive(true);
            quirkApplierBackImage.material.SetColor(startColorID, new Color(tmp2.r, tmp2.g, tmp2.b, 1.0f));
            quirkApplierBackImage.material.SetColor(endColorID, new Color(tmp2.r, tmp2.g, tmp2.b, 0.0f));
            quirkApplierQuirkDesc.text = $"[{tmp.data.appliedQuirk.quirkName.text}]: {tmp.data.appliedQuirk.quirkDescription.text}";
        }
        else quirkApplierDisplay.SetActive(false);
        undroppable.SetActive(!item.data.canDrop);
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
    private void OnDestroy()
    {
        if (grabbingSlot.item != null) grabbingSlot.item.Delete();
    }
}
public enum InventoryGrabMode
{
    Click,
    Drag
}