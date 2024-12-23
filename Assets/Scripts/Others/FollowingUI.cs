using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingUI : MonoBehaviour
{
    [SerializeField] Transform target;
    void LateUpdate()
    {
        transform.position = Camera.main.WorldToScreenPoint(target.position);
    }
}
