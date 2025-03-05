using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderEnterTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent triggered;
    [SerializeField] string[] targetTags;

    private void OnTriggerEnter(Collider other)
    {
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