using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Glock_Model : PistolModel
{
    [SerializeField] GameObject m_scope;
    public GameObject scope => m_scope;
}