using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabOpener : MonoBehaviour
{
    Tab openTab;
    public void OpenTab(Tab tab)
    {
        if (openTab != null) openTab.CloseTab();
        if (openTab != tab) openTab = tab;
        else openTab = null;
        if (openTab != null) openTab.OpenTab();
    }
    public void CloseTab()
    {
        if (openTab != null) openTab.CloseTab();
        openTab = null;
    }
}
