using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroppedItem : PooledPrefab<DroppedItem>
{
    [SerializeField] Transform anchor;
    Item displaying;
    ItemModel displayingModel;
    public Rigidbody rb { get; private set; }
    static DroppedItem prefab;

    public InventorySlot slot { get; } = new();
    public static DroppedItem Create(Item item, int count)
    {
        if (prefab == null) prefab = Resources.Load<DroppedItem>("DroppedItem");
        DroppedItem tmp = prefab.Instantiate();
        tmp.Set(item, count);
        return tmp;
    }
    protected override void OnCreate()
    {
        base.OnCreate();
        slot.onItemChange += OnItemChange;
        rb = GetComponent<Rigidbody>();
    }
    void Set(Item item, int count)
    {
        slot.SetItem(item);
        slot.count = count;
    }
    void OnItemChange()
    {
        if (slot.item == null) Release();
        else
        {
            if(displaying != null)
            {
                displaying.onModelUpdate -= OnModelUpdate;
            }
            displaying = slot.item;
            displaying.onModelUpdate += OnModelUpdate;
            OnModelUpdate();
        }
    }
    void OnModelUpdate()
    {
        if (displayingModel != null) displayingModel.Release();

        ItemModel tmp = displaying.GetModel();
        tmp.transform.SetParent(anchor, false);
        tmp.transform.localPosition = Vector3.zero;
        displayingModel = tmp;
    }
}
