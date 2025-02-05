using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tab : MonoBehaviour
{
    public virtual void OpenTab()
    {
        gameObject.SetActive(true);
    }
    public virtual void CloseTab()
    {
        gameObject.SetActive(false);
    }
}
