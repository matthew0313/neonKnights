using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DroppedItemManager : MonoBehaviour, ISavable
{
    public static DroppedItemManager Instance { get; private set; }
    public DroppedItemManager() => Instance = this;
    List<DroppedItem> droppedItems = new();
    public void Add(DroppedItem item)
    {
        droppedItems.Add(item);
    }
    public void Remove(DroppedItem item)
    {
        droppedItems.Remove(item);
    }

    public void Save(SaveData data)
    {
        data.droppedItems[GameManager.Instance.stageIndex] = new();
        foreach (DroppedItem item in droppedItems)
        {
            data.droppedItems[GameManager.Instance.stageIndex].saves.Add(item.Save());
        }
    }

    public void Load(SaveData data)
    {
        if(data.droppedItems.TryGetValue(GameManager.Instance.stageIndex, out DroppedItemManagerSave tmp))
        {
            foreach (DroppedItemSave save in tmp.saves)
            {
                DroppedItem item = DroppedItem.CreateDroppedItem();
                item.Load(save);
                droppedItems.Add(item);
            }
        }
    }
}
[System.Serializable]
public class DroppedItemManagerSave
{
    public List<DroppedItemSave> saves = new();
}