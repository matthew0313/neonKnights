using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public void ToTitle()
    {
        GlobalManager.Instance.LoadScene("Title");
    }
}
