using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemManager : MonoBehaviour
{
    static ItemManager instance;
    public static ItemManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameObject().AddComponent<ItemManager>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }
    List<Item> items = new(), addQueue = new(), removeQueue = new();
    public float itemRenderSpaceRotation = 0.0f;
    public void AddItem(Item item)
    {
        addQueue.Add(item);
    }
    public void RemoveItem(Item item)
    {
        removeQueue.Add(item);
    }
    const float itemRenderRotateSpeed = 90.0f;
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
        itemRenderSpaceRotation += Time.deltaTime * itemRenderRotateSpeed;
        if (itemRenderSpaceRotation > 360.0f) itemRenderSpaceRotation -= 360.0f;
    }
}