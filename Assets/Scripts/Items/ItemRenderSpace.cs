using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemRenderSpace : PooledPrefab<ItemRenderSpace>
{
    [SerializeField] Camera camera;
    [SerializeField] Transform itemAnchor;
    ItemModel rendering;

    static ItemRenderSpace prefab;
    static int count = 0;
    public static ItemRenderSpace GetSpace()
    {
        if (prefab == null) prefab = Resources.Load<ItemRenderSpace>("ItemRenderSpace");
        ItemRenderSpace tmp = prefab.Instantiate();
        DontDestroyOnLoad(tmp);
        tmp.transform.position = new Vector3(-2000.0f + -100.0f * count++, -2000.0f, 0.0f);
        return tmp;
    }
    public void SetTarget(RenderTexture output) => camera.targetTexture = output;
    public void RenderItem(ItemModel item)
    {
        if (rendering != null) rendering.Release();
        rendering = item;
        item.transform.SetParent(itemAnchor, false);
        item.transform.localPosition = Vector3.zero;
    }
    private void Update()
    {
        itemAnchor.transform.rotation = Quaternion.Euler(ItemManager.Instance.itemRenderSpaceRotation, 90.0f, 90.0f);
    }
}