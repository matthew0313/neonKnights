using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiverInteraction : Interaction, ISavable
{
    [SerializeField] ItemData givenItem;
    [SerializeField] int givenCount = 1;
    [SerializeField] GameObject model;
    [SerializeField] SaveID id;
    bool given = false;
    public override LangText interactionName => new LangText()
    {
        en = $"Pick up {givenItem.itemName.text}{(givenCount > 1 ? $"({givenCount})" : null)}",
        kr = $"{givenItem.itemName.text}{(givenCount > 1 ? $"({givenCount})" : null)} 줍기"
    };
    Item item;
    public override void OnInteract()
    {
        if (item == null) item = givenItem.Create();
        if (player.inventory.TryInsert(item, givenCount) == 0)
        {
            player.inventory.Insert(item, givenCount);
            given = true;
            model.SetActive(false);
            enabled = false;
        } 
    }
    public void Save(SaveData data)
    {
        data.mapElements[id.value] = new();
        data.mapElements[id.value].bools["Given"] = given;
    }
    public void Load(SaveData data)
    {
        if(data.mapElements.TryGetValue(id.value, out DataUnit tmp))
        {
            if (tmp.bools.TryGetValue("Given", out bool val))
            {
                model.SetActive(!val);
                enabled = !val;
                given = val;
            }
        }
    }
}