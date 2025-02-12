using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public static class TweenLibrary
{
    public static Tween SimpleProgress(float duration, Action<float> onUpdate)
    {
        float progress = 0.0f;
        return DOTween.To(() => progress, value =>
        {
            progress = value;
            onUpdate?.Invoke(progress);
        }, 1.0f, duration);
    }
}