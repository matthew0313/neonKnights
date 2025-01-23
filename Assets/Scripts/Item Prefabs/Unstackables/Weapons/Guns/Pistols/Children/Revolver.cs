using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Revolver : Pistol
{
    [SerializeField] GameObject m_scope;
    public GameObject scope => m_scope;
}