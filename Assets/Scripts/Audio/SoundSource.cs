using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundSource : MonoBehaviour
{
    [SerializeField] Sound sound;
    [SerializeField] bool ignoreSettings = false;
    [SerializeField] int maxSourceCount = 10;
    List<AudioSource> sources = new();
    public AudioSource baseSource { get; private set; }
    private void Awake()
    {
        baseSource = GetComponent<AudioSource>();
        baseSource.clip = sound.clip;
        baseSource.volume = sound.volume * (ignoreSettings ? Settings.volume : 1);
        sources.Add(baseSource);
    }
    private void OnEnable()
    {
        if (ignoreSettings) return;
        Settings.onVolumeChange += OnVolumeChange;
    }
    private void OnDisable()
    {
        if (ignoreSettings) return;
        Settings.onVolumeChange -= OnVolumeChange;
    }
    void OnVolumeChange()
    {
        foreach (var i in sources) i.volume = sound.volume * (ignoreSettings ? Settings.volume : 1);
    }
    public void PlaySound()
    {
        if (sources[0].isPlaying && sources.Count < maxSourceCount)
        {
            AudioSource tmp = gameObject.AddComponent<AudioSource>();
            ApplySettings(tmp);
            sources.Insert(0, tmp);
        }
        AudioSource source = sources[0];
        source.Play();
        sources.RemoveAt(0);
        sources.Add(source);
    }
    void ApplySettings(AudioSource source)
    {
        source.clip = sound.clip;
        source.volume = sound.volume * (ignoreSettings ? Settings.volume : 1);
        source.bypassEffects = baseSource.bypassEffects;
        source.bypassListenerEffects = baseSource.bypassListenerEffects;
        source.bypassReverbZones = baseSource.bypassReverbZones;
    }
}
[System.Serializable]
public struct Sound
{
    public AudioClip clip;
    public float volume;
}