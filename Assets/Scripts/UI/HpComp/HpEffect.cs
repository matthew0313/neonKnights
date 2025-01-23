using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

public class HpEffect : MonoBehaviour
{
    [SerializeField] HpComp origin;
    static HpEffectText effectText;
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
        if (effectText == null) effectText = Resources.Load<HpEffectText>("HpEffectText");
        effectText.Instantiate().Set(origin.transform.position, info);
    }
}