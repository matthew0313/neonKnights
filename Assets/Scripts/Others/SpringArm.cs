using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringArm : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] LayerMask whatToHit;
    public float armLength = 3.0f;
    private void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward * -1.0f, out RaycastHit hit, armLength, whatToHit))
        {
            target.transform.position = hit.point;
        }
        else target.transform.position = transform.position + transform.forward * -1.0f * armLength;
    }
}
