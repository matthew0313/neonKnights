using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingUI : MonoBehaviour
{
    public void ToTitle()
    {
        GlobalManager.Instance.LoadScene("Title");
    }
}
