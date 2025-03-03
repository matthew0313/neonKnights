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

    public static DroppedItem CreateDroppedItem()
    {
        if (prefab == null) prefab = Resources.Load<DroppedItem>("DroppedItem");
        DroppedItem tmp = prefab.Instantiate();
        return tmp;
    }
    public static DroppedItem CreateDroppedItem(Item item, int count)
    {
        DroppedItem tmp = CreateDroppedItem();
        tmp.Set(item, count);
        return tmp;
    }
    private void OnEnable()
    {
        DroppedItemManager.Instance.Add(this);
    }
    private void OnDisable()
    {
        DroppedItemManager.Instance.Remove(this);
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
    public DroppedItemSave Save()
    {
        return new DroppedItemSave
        {
            position = transform.position,
            velocity = rb.linearVelocity,
            slot = slot.Save()
        };
    }
    public void Load(DroppedItemSave data)
    {
        transform.position = data.position;
        rb.linearVelocity = data.velocity;
        slot.Load(data.slot);
    }
}
[System.Serializable]
public class DroppedItemSave
{
    public Vector3 position;
    public Vector3 velocity;
    public InventorySlotSaveData slot;
}