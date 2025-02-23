using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class GravityCharacterController : MonoBehaviour
{
    CharacterController m_origin;
    CharacterController origin
    {
        get
        {
            if(m_origin == null) m_origin = GetComponent<CharacterController>();
            return m_origin;
        }
    }
    const float gravity = -9.81f;
    float downVelocity = 0.0f;

    public bool isGrounded => origin.isGrounded;
    private void OnEnable()
    {
        downVelocity = 0.0f;
    }
    void Update()
    {
        if (isGrounded)
        {
            downVelocity = Mathf.Max(0.0f, downVelocity);
        }
        else
        {
            downVelocity += gravity * Time.deltaTime;
        }
        Move(Vector2.up * downVelocity * Time.deltaTime);
    }
    public void Move(Vector3 move)
    {
        origin.Move(move);
    }
}