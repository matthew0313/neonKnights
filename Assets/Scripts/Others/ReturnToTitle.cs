using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReturnToTitle : MonoBehaviour
{
    public void ToTitle()
    {
        GlobalManager.Instance.LoadScene("Title");
    }
}