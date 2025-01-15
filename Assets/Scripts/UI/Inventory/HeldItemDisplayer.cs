using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Text;
using UnityEditor.UIElements;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

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
        displayingItem = item;

        if(displaying != null) displaying.Release();
        if(item != null && item.data.model != null)
        {
            displaying = item.data.model.Instantiate();
            displaying.transform.SetParent(itemModelAnchor, false);
            displaying.transform.localPosition = Vector3.zero;
        }

        if(item is IReloadDisplayed)
        {
            UpdateReloadDisplay();
        }
        else reloadDisplay.SetActive(false);
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