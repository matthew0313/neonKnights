using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : PooledPrefab<ExplosionEffect>
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