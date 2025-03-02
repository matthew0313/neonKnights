using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public void StartGame()
    {
        GlobalManager.Instance.LoadScene("Stage 1");
    }
}
