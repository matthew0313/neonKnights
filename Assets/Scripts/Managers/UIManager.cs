using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, ICutsceneCallbackReceiver
{
    public static UIManager Instance { get; private set; }
    public UIManager()
    {
        Instance = this;
    }
    [SerializeField] GameObject defaultUI;

    [Header("Tabs")]
    [SerializeField] InventoryTab inventoryTab;

    [Header("Pause UI")]
    [SerializeField] GameObject pauseUI;
    Tab currentOpenTab;
    public bool isTabOpen => currentOpenTab != null;

    ControlMachine cursorLockControlMachine;
    ControlElement tabOpenCursorControl, cutsceneCursorControl, pauseCursorControl;
    private void Awake()
    {
        inventoryTab.OnAwake();

        cursorLockControlMachine = new(() => Cursor.lockState = CursorLockMode.Locked);
        tabOpenCursorControl = new(1, () => Cursor.lockState = CursorLockMode.None, false);
        cutsceneCursorControl = new(2, () => Cursor.lockState = CursorLockMode.None, false);
        pauseCursorControl = new(9999, () => Cursor.lockState = CursorLockMode.None, false);
        cursorLockControlMachine.AddControls(tabOpenCursorControl, cutsceneCursorControl, pauseCursorControl);
    }
    private void Start()
    {
        GameManager.Instance.onPauseToggle += OnPauseToggle;
    }
    void OnPauseToggle(bool pause)
    {
        pauseUI.SetActive(pause);
        if (pause)
        {
            pauseUI.SetActive(true);
            pauseCursorControl.enabled = true;
        }
        else
        {
            pauseUI.SetActive(false);
            pauseCursorControl.enabled = false;
        }
    }
    public void ToggleTab(Tab tab)
    {
        if (currentOpenTab != null) currentOpenTab.CloseTab();

        if (currentOpenTab == tab) currentOpenTab = null;
        else currentOpenTab = tab;

        if (currentOpenTab != null)
        {
            currentOpenTab.OpenTab();
            tabOpenCursorControl.enabled = true;
        }
        else
        {
            tabOpenCursorControl.enabled = false;
        }
    }
    public void CloseTab()
    {
        ToggleTab(null);
    }
    bool canControl = true;
    private void Update()
    {
        if (!canControl) return;
        if (InputManager.GetInventoryTabInput()) ToggleTab(inventoryTab);
    }
    public void OnCutsceneStart()
    {
        canControl = false;
        defaultUI.SetActive(false);
        CloseTab();
        cutsceneCursorControl.enabled = true;
    }

    public void OnCutsceneEnd()
    {
        canControl = true;
        defaultUI.SetActive(true);
        cutsceneCursorControl.enabled = false;
    }
    void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}