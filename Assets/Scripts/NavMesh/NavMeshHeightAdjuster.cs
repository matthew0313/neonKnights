using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NavMeshHeightAdjuster : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform adjusted;

    [Header("Detection")]
    [SerializeField] LayerMask detectedMask;
    [SerializeField] float detectionOffset = 1.0f;
    [SerializeField] float ceilingDistance, floorDistance;

    [Header("Movement")]
    [SerializeField] float heightAdjustSpeed = 1.0f;
    [SerializeField] float allowedRange = 0.5f;
    private void Update()
    {
        Vector3 detectPos = agent.transform.position + agent.velocity * detectionOffset;
        float desiredHeight = adjusted.position.y;
        if (Physics.Raycast(detectPos, Vector3.down, out RaycastHit hit2, Mathf.Infinity, detectedMask))
        {
            desiredHeight = Mathf.Max(desiredHeight, hit2.point.y + floorDistance);
        }
        if (Physics.Raycast(detectPos, Vector3.up, out RaycastHit hit1, Mathf.Infinity, detectedMask))
        {
            desiredHeight = Mathf.Min(desiredHeight, hit1.point.y - ceilingDistance);
        }

        if (adjusted.position.y < desiredHeight - allowedRange)
        {
            adjusted.position += Vector3.up * heightAdjustSpeed * Time.deltaTime;
        }
        else if(adjusted.position.y > desiredHeight + allowedRange)
        {
            adjusted.position -= Vector3.up * heightAdjustSpeed * Time.deltaTime;
        }
    }
}