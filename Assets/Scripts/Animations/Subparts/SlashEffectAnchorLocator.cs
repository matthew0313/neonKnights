using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashEffectAnchorLocator : AnimatorSubpart
{
    [SerializeField] Transform m_slashEffectAnchor;
    public Transform slashEffectAnchor => m_slashEffectAnchor;
}