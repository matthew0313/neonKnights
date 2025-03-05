using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NavMeshAnimatorVelocity : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] Transform pivot;
    [SerializeField] string axisX = "MoveX", axisY = "MoveY";
    int axisXID, axisYID;
    private void Awake()
    {
        axisXID = Animator.StringToHash(axisX);
        axisYID = Animator.StringToHash(axisY);
    }
    private void Update()
    {
        Vector3 vel = pivot != null ? pivot.InverseTransformDirection(agent.velocity) : agent.velocity;
        vel.Normalize();
        animator.SetFloat(axisXID, vel.x);
        animator.SetFloat(axisYID, vel.z);
    }
}