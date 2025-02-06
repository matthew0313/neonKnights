using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SlashEffect : PooledPrefab<SlashEffect>
{
    [SerializeField] VisualEffect effect;
    [SerializeField] float duration = 1.0f;
    float counter = 0.0f;
    private void OnEnable()
    {
        effect.Reinit();
        effect.Play();
        counter = 0.0f;
    }
    private void Update()
    {
        counter += Time.deltaTime;
        if (counter >= duration) Release();
    }
}