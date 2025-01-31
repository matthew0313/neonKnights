using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : PooledPrefab<HitEffect>
{
    [SerializeField] ParticleSystem particle;
    private void OnEnable()
    {
        particle.Play();
    }
    private void Update()
    {
        if (!particle.isPlaying) Release();
    }
}