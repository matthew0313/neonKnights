using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaCharge : MonoBehaviour
{
    [SerializeField] Image m_image;
    public Image image => m_image;
    float currentScale = 1.0f;
    Tween tween;
    public void SetScale(float scale)
    {
        if(tween != null)
        {
            tween.Kill();
            tween = null;
        }
        if (scale < currentScale)
        {
            image.transform.localScale = new Vector2(scale, scale);
            currentScale = scale;
        }
        else if(scale > currentScale)
        {
            tween = DOTween.To(() => currentScale, value =>
            {
                image.transform.localScale = new Vector2(value, value);
                currentScale = value;
            }, scale, 0.2f).SetEase(Ease.OutCirc);
        }
    }
}