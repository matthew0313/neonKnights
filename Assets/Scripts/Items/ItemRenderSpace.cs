using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRenderSpace : PooledPrefab<ItemRenderSpace>
{
    [SerializeField] Camera camera;
    [SerializeField] float cameraUpdateTickRate = 0.1f;
    [SerializeField] Transform itemAnchor;
    ItemModel rendering;

    static ItemRenderSpace prefab;
    static int count = 0;
    protected override void OnCreate()
    {
        base.OnCreate();
        //DontDestroyOnLoad(gameObject);
        transform.position = new Vector3(-2000.0f + -100.0f * count, -2000.0f, 0.0f);
        gameObject.name = $"RenderSpace {count}";
        count++;
    }
    public static ItemRenderSpace GetSpace()
    {
        if (prefab == null) prefab = Resources.Load<ItemRenderSpace>("ItemRenderSpace");
        return prefab.Instantiate();
    }
    public void SetTarget(RenderTexture output) => camera.targetTexture = output;
    public void RenderItem(ItemModel item)
    {
        if (rendering != null) rendering.Release();
        rendering = item;
        item.transform.SetParent(itemAnchor, false);
        item.transform.localPosition = Vector3.zero;
    }
    float cameraUpdateCounter = 0.0f;
    private void Update()
    {
        itemAnchor.transform.rotation = Quaternion.Euler(ItemManager.Instance.itemRenderSpaceRotation, 90.0f, 90.0f);
        cameraUpdateCounter += Time.deltaTime;
        if(cameraUpdateCounter >= cameraUpdateTickRate)
        {
            camera.Render();
            cameraUpdateCounter = 0.0f;
        }
    }
}