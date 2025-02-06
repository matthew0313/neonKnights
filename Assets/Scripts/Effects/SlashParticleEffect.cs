using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SlashParticleEffect : SlashEffect
{
    [SerializeField] ParticleSystem effect;
    private void OnEnable()
    {
        effect.Play();
    }
    private void Update()
    {
        if (!effect.isPlaying) Release();
    }
}