using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Knife : Melee
{
    [SerializeField] MeshRenderer mesh;
    [SerializeField] Material normalMaterial, cursedMaterial;
    public void SetCurse(bool curse)
    {
        mesh.material = curse ? cursedMaterial : normalMaterial;
    }
}