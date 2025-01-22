using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Gun : Weapon
{
    [SerializeField] Transform m_firePoint;
    public Transform firePoint => m_firePoint;
}