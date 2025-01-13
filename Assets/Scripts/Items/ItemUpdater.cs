using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemUpdater : MonoBehaviour
{
    static ItemUpdater instance;
    public static ItemUpdater Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameObject().AddComponent<ItemUpdater>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }
    List<Item> items = new(), addQueue = new(), removeQueue = new();
    public void AddItem(Item item)
    {
        addQueue.Add(item);
    }
    public void RemoveItem(Item item)
    {
        removeQueue.Add(item);
    }
    private void Update()
    {
        foreach(var i in items)
        {
            i.Update();
        }
        foreach(var i in addQueue)
        {
            items.Add(i);
        }
        foreach(var i in removeQueue)
        {
            items.Remove(i);
        }
    }
}