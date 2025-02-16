using UnityEngine;

public class HeldItemDisplayer : MonoBehaviour
{
    [SerializeField] Transform itemModelAnchor;

    [Header("Reload Display")]
    [SerializeField] GameObject reloadDisplay;
    [SerializeField] Transform reloadDisplayBar;
    Item displayingItem;
    ItemModel displaying;

    Player_Inventory player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Inventory>();
        player.onEquippedItemChange += OnEquippedItemChange;
    }
    void OnEquippedItemChange(Item item)
    {
        if (displayingItem != null) displayingItem.onModelUpdate -= OnDisplayingItemModelUpdate;
        displayingItem = item;
        if(displayingItem != null) displayingItem.onModelUpdate += OnDisplayingItemModelUpdate;

        if(displaying != null) displaying.Release();
        if(item != null)
        {
            displaying = item.GetModel();
            displaying.transform.SetParent(itemModelAnchor, false);
            displaying.transform.localPosition = Vector3.zero;
        }

        if(item is IReloadDisplayed)
        {
            UpdateReloadDisplay();
        }
        else reloadDisplay.SetActive(false);
    }
    void OnDisplayingItemModelUpdate()
    {
        displaying.Release();
        displaying = displayingItem.GetModel();
        displaying.transform.SetParent(itemModelAnchor, false);
        displaying.transform.localPosition = Vector3.zero;
    }
    private void Update()
    {
        if(displayingItem != null)
        {
            if(displayingItem is IReloadDisplayed)
            {
                UpdateReloadDisplay();
            }
        }
    }
    void UpdateReloadDisplay()
    {
        if ((displayingItem as IReloadDisplayed).reloading)
        {
            reloadDisplay.SetActive(true);
            reloadDisplayBar.localScale = new Vector2(1.0f - (displayingItem as IReloadDisplayed).reloadProgress, 1.0f);
        }
        else reloadDisplay.SetActive(false);
    }
}