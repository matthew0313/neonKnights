using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public UIManager()
    {
        Instance = this;
    }

    [Header("Tabs")]
    [SerializeField] InventoryTab inventoryTab;
    Tab currentOpenTab = null;
    private void Awake()
    {
        inventoryTab.OnAwake();
    }
    public void ToggleTab(Tab tab)
    {
        if (currentOpenTab != null) currentOpenTab.CloseTab();
        if (currentOpenTab == tab) currentOpenTab = null;
        else currentOpenTab = tab;
        if (currentOpenTab != null) currentOpenTab.OpenTab();
    }
    public void CloseTab()
    {
        ToggleTab(null);
    }
    private void Update()
    {
        if (InputManager.GetInventoryTabInput()) ToggleTab(inventoryTab);
    }
}