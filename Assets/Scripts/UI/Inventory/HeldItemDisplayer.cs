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
    ItemModel displaying;

    Player_Inventory player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Inventory>();
        player.onEquippedItemChange += OnEquippedItemChange;
    }
    void OnEquippedItemChange(Item item)
    {
        if(displaying != null) displaying.Release();
        if(item != null && item.data.model != null)
        {
            displaying = item.data.model.Instantiate();
            displaying.transform.SetParent(itemModelAnchor, false);
            displaying.transform.localPosition = Vector3.zero;
        }
    }
}