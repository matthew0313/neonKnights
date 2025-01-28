using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class TriggerCallbacks : MonoBehaviour
{
    public Action<Collider> onTriggerEnter, onTriggerStay, onTriggerExit;
    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(other);
    }
    private void OnTriggerStay(Collider other)
    {
        onTriggerStay?.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke(other);
    }
}