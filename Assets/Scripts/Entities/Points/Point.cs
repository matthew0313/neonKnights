using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    [SerializeField] GameObject m_pointOf;
    public GameObject pointOf => m_pointOf;
}