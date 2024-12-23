using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCam : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float lerpRate;
    private void FixedUpdate()
    {
        if (target == null) return;
        transform.position = Vector3.Lerp(transform.position, target.position, lerpRate * Time.fixedDeltaTime);
    }
}
