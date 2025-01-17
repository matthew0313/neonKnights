using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(TMP_Text), typeof(RainbowText))]
public class HpEffectText : PooledPrefab<HpEffectText>
{
    [SerializeField] float disappearTime = 0.5f, moveRange = 100.0f;
    TMP_Text text;
    RainbowText rainbow;
    protected override void OnCreate()
    {
        base.OnCreate();
        rainbow = GetComponent<RainbowText>();
        text = GetComponent<TMP_Text>();
    }
    public void Set(string text, Vector3 position, Color color, bool isRainbow = false)
    {
        rainbow.enabled = isRainbow;
        this.text.text = text;
        this.text.color = color;

        transform.SetParent(MainCanvas.Instance.transform);

        float angle = Random.Range(0.0f, 360.0f);
        Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.0f);

        transform.position = Camera.main.WorldToScreenPoint(position);
        float counter = 0.0f;

        DOTween.To(() => counter, value =>
        {
            counter = value;
            transform.position = Camera.main.WorldToScreenPoint(position);
            transform.localPosition += dir * counter * moveRange;
            this.text.color = new Color(this.text.color.r, this.text.color.g, this.text.color.b, 1.0f - counter);
        }, 1.0f, disappearTime).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            Release();
        });
    }
}