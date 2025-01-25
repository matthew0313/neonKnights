using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuirkShard_Model : ItemModel
{
    [SerializeField] new MeshRenderer renderer;
    bool copied = false;
    readonly int colorID = Shader.PropertyToID("_Color");
    public void SetColor(Color color)
    {
        if (!copied)
        {
            renderer.material = new Material(renderer.material);
            copied = true;
        }
        renderer.material.SetColor(colorID, color);
    }
}