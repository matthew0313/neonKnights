using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

public class TextIndicator : PooledPrefab<TextIndicator>
{
    [SerializeField] TMP_Text text;
    [SerializeField] float moveUpDistance = 100;
    [SerializeField] float moveUpTime = 1.0f;
    static TextIndicator prefab;
    public static void Indicate(Vector3 position, string text, Color color)
    {
        if (MainCanvas.Instance == null) return;
        if (prefab == null) prefab = Resources.Load<TextIndicator>("TextIndicator");
        TextIndicator tmp = prefab.Instantiate();
        tmp.transform.SetParent(MainCanvas.Instance.transform, false);
        tmp.transform.localScale = Vector3.one;
        tmp.transform.position = Camera.main.WorldToScreenPoint(position);
        tmp.text.text = text;
        tmp.text.color = color;
        float counter = 0.0f;
        DOTween.To(() => counter, value => counter = value, 1.0f, tmp.moveUpTime).OnUpdate(() =>
        {
            tmp.transform.position = Camera.main.WorldToScreenPoint(position);
            tmp.transform.localPosition += Vector3.up * tmp.moveUpDistance * counter;
        }).OnComplete(() =>
        {
            tmp.Release();
        });
    }
}