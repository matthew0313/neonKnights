using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiamondRing_Model : AccessoryModel
{
    [SerializeField] MeshRenderer mesh;
    [SerializeField] Material normalMaterial, cursedMaterial;
    public void SetCurse(bool curse)
    {
        mesh.material = curse ? cursedMaterial : normalMaterial;
    }
}