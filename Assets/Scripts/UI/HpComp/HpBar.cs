using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

public class HpBar : MonoBehaviour
{
    [SerializeField] HpComp target;
    [SerializeField] Image hpBar, shieldBar, lossBar;

    float defaultHpBarWidth;
    float currentScale;
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
    Tween lossBarTween;
    void OnHpChange()
    {
        hpBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultHpBarWidth * target.hp / target.baseHp);
        shieldBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultHpBarWidth * target.shield / target.baseHp);
        float targetScale = target.hp / target.baseHp + target.shield / target.baseHp;
        if (targetScale < currentScale)
        {
            if (lossBarTween != null) lossBarTween.Kill();
            lossBarTween = DOTween.To(() => currentScale, value => currentScale = value, targetScale, 3.0f).SetDelay(1.0f).OnUpdate(() => 
            {
                Debug.Log(currentScale);
                lossBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultHpBarWidth * currentScale); 
            });
        }
        else if(targetScale > currentScale)
        {
            currentScale = targetScale;
            lossBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultHpBarWidth * currentScale);
        }
    }
}