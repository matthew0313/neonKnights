using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderStayTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent triggered;
    [SerializeField] string[] targetTags;

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(name);
        if (!enabled) return;
        foreach(var i in targetTags)
        {
            if (other.CompareTag(i))
            {
                triggered.Invoke();
                return;
            }
        }
    }
}