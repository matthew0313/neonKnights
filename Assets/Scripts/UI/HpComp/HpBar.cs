using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] HpComp target;
    [SerializeField] Image hpBar, shieldBar;

    float defaultHpBarWidth;
    private void Awake()
    {
        defaultHpBarWidth = hpBar.rectTransform.rect.width;
    }
    private void OnEnable()
    {
        target.onHpChange += OnHpChange;
        OnHpChange();
    }
    private void OnDisable()
    {
        target.onHpChange -= OnHpChange;
    }
    void OnHpChange()
    {
        hpBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultHpBarWidth * target.hp / target.baseHp);
        shieldBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultHpBarWidth * target.shield / target.baseHp);
    }
}