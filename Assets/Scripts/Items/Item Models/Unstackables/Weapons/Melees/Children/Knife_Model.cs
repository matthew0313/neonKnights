using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Knife_Model : MeleeModel
{
    [SerializeField] MeshRenderer mesh;
    [SerializeField] Material normalMaterial, cursedMaterial;
    public void SetCurse(bool curse)
    {
        mesh.material = curse ? cursedMaterial : normalMaterial;
    }
}