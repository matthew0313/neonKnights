using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryBeltUI : MonoBehaviour
{
    [SerializeField] InventorySlotUI[] slots = new InventorySlotUI[Player_Inventory.beltLength];
    [SerializeField] Transform equippedSlotIndicator;
    Player_Inventory origin;
    private void Awake()
    {
        origin = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Inventory>();
    }
    private void Start()
    {
        for(int i = 0; i < Player_Inventory.beltLength; i++)
        {
            slots[i].Set(origin.slots[i]);
        }
    }
    private void LateUpdate()
    {
        equippedSlotIndicator.transform.position = slots[origin.equippedSlotIndex].transform.position;
    }
}
