using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] RawImage icon;
    [SerializeField] TMP_Text count;
    [SerializeField] Image emptyImage;

    [Header("Bar")]
    [SerializeField] GameObject bar;
    [SerializeField] Image barScaler;

    [Header("Number")]
    [SerializeField] GameObject number;
    [SerializeField] TMP_Text numberText;

    [Header("Appliables")]
    [SerializeField] Image canApply;
    [SerializeField] Image cannotApply;

    Item displaying;
    public InventorySlot origin { get; private set; }
    public void Set(InventorySlot origin)
    {
        this.origin = origin;
        origin.onItemChange += OnItemChange;
        origin.onCountChange += OnCountChange;
        OnItemChange();
        OnCountChange();
    }
    protected virtual void OnItemChange()
    {
        if(displaying is INumberDisplayed)
        {
            (displaying as INumberDisplayed).onNumberUpdate -= OnNumberUpdate;
        }

        displaying = origin.item;

        if(displaying != null)
        {
            icon.texture = displaying.render;
            //icon.sprite = displaying.data.icon;
            icon.gameObject.SetActive(true);
            if(emptyImage != null) emptyImage.gameObject.SetActive(false);
        }
        else
        {
            icon.gameObject.SetActive(false);
            if (emptyImage != null) emptyImage.gameObject.SetActive(true);
        }

        if (displaying is IBarDisplayed)
        {
            BarUpdate();
        }
        else bar.SetActive(false);

        if(displaying is INumberDisplayed)
        {
            number.SetActive(true);
            (displaying as INumberDisplayed).onNumberUpdate += OnNumberUpdate;
            OnNumberUpdate();
        }
        else number.SetActive(false);
        AppliableStateUpdate();
    }
    void OnCountChange()
    {
        if(origin.count <= 1 || origin.item == null)
        {
            count.gameObject.SetActive(false);
        }
        else
        {
            count.text = $"{origin.count}{origin.item.data.unit}";
            count.gameObject.SetActive(true);
        }
        AppliableStateUpdate();
    }
    void LateUpdate()
    {
        if (displaying is IBarDisplayed)
        {
            BarUpdate();
        }
        AppliableStateUpdate();
    }
    void BarUpdate()
    {
        bar.gameObject.SetActive((displaying as IBarDisplayed).barDisplay);
        barScaler.transform.localScale = new Vector2((displaying as IBarDisplayed).barFill, 1.0f);
        barScaler.color = (displaying as IBarDisplayed).barColor;
    }
    void OnNumberUpdate()
    {
        numberText.text = (displaying as INumberDisplayed).numberText;
    }
    void AppliableStateUpdate()
    {
        if (origin == null) return;
        if(origin.item != null && InventorySlotUIHandler.Instance != null && InventorySlotUIHandler.Instance.grabbingSlot.item != null && InventorySlotUIHandler.Instance.grabbingSlot.item is IApplied tmp)
        {
            bool appliable = tmp.IsAppliableTo(origin.item);
            canApply.gameObject.SetActive(appliable);
            cannotApply.gameObject.SetActive(!appliable); 
        }
        else
        {
            canApply.gameObject.SetActive(false);
            cannotApply.gameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        if(origin != null)
        {
            origin.onItemChange -= OnItemChange;
            origin.onCountChange -= OnCountChange;
        }
    }
}
