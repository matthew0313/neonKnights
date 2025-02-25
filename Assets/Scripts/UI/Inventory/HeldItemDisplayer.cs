using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeldItemDisplayer : MonoBehaviour
{
    [Header("Held Item Display")]
    [SerializeField] Transform itemModelAnchor;
    [SerializeField] TMP_Text heldItemName;

    [Header("Number Display")]
    [SerializeField] TMP_Text number;

    [Header("Reload Display")]
    [SerializeField] GameObject reloadDisplay;
    [SerializeField] Image reloadDisplayBar;
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
        if(displayingItem is INumberDisplayed)
        {
            (displayingItem as INumberDisplayed).onNumberUpdate -= OnNumberUpdate;
        }

        displayingItem = item;

        if (displayingItem != null)
        {
            displayingItem.onModelUpdate += OnDisplayingItemModelUpdate;
            OnDisplayingItemModelUpdate();

            heldItemName.text = item.data.itemName.text;
            heldItemName.gameObject.SetActive(true);
        }
        else
        {
            heldItemName.gameObject.SetActive(false);
            if(displaying != null) displaying.Release();
        }
        
        if(item is INumberDisplayed)
        {
            number.gameObject.SetActive(true);
            (displayingItem as INumberDisplayed).onNumberUpdate += OnNumberUpdate;
            OnNumberUpdate();
        }
        else
        {
            number.gameObject.SetActive(false);
        }

        if(item is IReloadDisplayed)
        {
            UpdateReloadDisplay();
        }
        else reloadDisplay.SetActive(false);
    }
    void OnDisplayingItemModelUpdate()
    {
        if(displaying != null) displaying.Release();
        displaying = displayingItem.GetModel();
        displaying.transform.SetParent(itemModelAnchor, false);
        displaying.transform.localPosition = Vector3.zero;
    }
    void OnNumberUpdate()
    {
        number.text = (displayingItem as INumberDisplayed).numberText;
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
            reloadDisplayBar.fillAmount = 1.0f - (displayingItem as IReloadDisplayed).reloadProgress;
        }
        else reloadDisplay.SetActive(false);
    }
}