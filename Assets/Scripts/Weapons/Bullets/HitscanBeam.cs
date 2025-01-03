using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitscanBeam : HitscanEffect
{
    [SerializeField] float disappearTime = 0.3f;
    public override void Set(float range)
    {
        counter = 0.0f;
        transform.localScale = new Vector3(1, 1, range);
    }
    float counter = 0.0f;
    private void Update()
    {
        counter += Time.deltaTime;
        if (counter >= disappearTime) Release();
    }
}