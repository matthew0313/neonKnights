using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

public class HpEffect : MonoBehaviour
{
    [SerializeField] HpComp origin;
    private void OnEnable()
    {
        origin.onDamage += OnDamage;
    }
    private void OnDisable()
    {
        origin.onDamage -= OnDamage;
    }
    void OnDamage(DamageReceivedInfo info)
    {
        HpEffectText.CreateNew().Set(transform.position, info);
    }
}