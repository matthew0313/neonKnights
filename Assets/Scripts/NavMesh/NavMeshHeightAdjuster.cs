using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NavMeshHeightAdjuster : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;

    [Header("Detection")]
    [SerializeField] LayerMask detectedMask;
    [SerializeField] float detectionOffset = 1.0f;
    [SerializeField] float ceilingDistance, floorDistance;

    [Header("Movement")]
    [SerializeField] float heightAdjustSpeed = 1.0f;
    private void Update()
    {
        RaycastHit hit;

        Vector3 detectPos = transform.position + agent.velocity * detectionOffset;
        float desiredHeight = transform.position.y;
        if (Physics.Raycast(detectPos, Vector3.up, out hit, ceilingDistance, detectedMask))
        {
            desiredHeight = hit.point.y - ceilingDistance;
        }
        else if (Physics.Raycast(detectPos, Vector3.down, out hit, Mathf.Infinity, detectedMask))
        {
            desiredHeight = hit.point.y + floorDistance;
        }

        if(transform.position.y < desiredHeight)
        {
            transform.position += Vector3.up * heightAdjustSpeed * Time.deltaTime;
        }
        else if(transform.position.y > desiredHeight)
        {
            transform.position -= Vector3.up * heightAdjustSpeed * Time.deltaTime;
        }
    }
}