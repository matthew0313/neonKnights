using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameObject().AddComponent<AudioManager>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }
    Pooler<AudioSource> audioSourcePool = new(() =>
    {
        return new GameObject("PlayingAudio").AddComponent<AudioSource>();
    });
    List<PlayingAudio> playing = new(), addQueue = new(), removeQueue = new();
    public PlayingAudio PlaySound(Sound sound)
    {
        if (sound.clip == null) return null;
        PlayingAudio tmp = new(sound, audioSourcePool.GetObject());
        tmp.source.clip = tmp.sound.clip;
        tmp.source.volume = tmp.sound.volume;
        tmp.source.Play();
        playing.Add(tmp);
        return tmp;
    }
    void Update()
    {
        foreach(var i in playing)
        {
            i.Update();
        }
        foreach (var i in removeQueue)
        {
            audioSourcePool.ReleaseObject(i.source);
            playing.Remove(i);
        }
        removeQueue.Clear();
    }
    public void RemoveAudio(PlayingAudio audio)
    {
        removeQueue.Add(audio);
    }
}
public static class PlayingAudioManagement
{
    public static PlayingAudio SetDist(this PlayingAudio audio, Func<Vector3> locationGetter, float maxDistance)
    {
        if (audio == null) return null;
        audio.distanceControl = true;
        audio.locationGetter = locationGetter;
        audio.maxDistance = maxDistance;
        audio.Update();
        return audio;
    }
    public static PlayingAudio Loop(this PlayingAudio audio)
    {
        if (audio == null) return null;
        audio.source.loop = true;
        return audio;
    }
    public static PlayingAudio Volume(this PlayingAudio audio, float volume)
    {
        if (audio == null) return null;
        audio.volume = volume;
        audio.Update();
        return audio;
    }
}
public class PlayingAudio
{
    public PlayingAudio(Sound sound, AudioSource source)
    {
        this.sound = sound;
        this.source = source;
    }
    public readonly Sound sound;
    public readonly AudioSource source;

    public float volume = 1.0f;

    public bool distanceControl = false;
    public Func<Vector3> locationGetter;
    public float maxDistance = 30;
    public void Update()
    {
        if (!source.isPlaying)
        {
            Stop();
            return;
        }
        source.volume = Settings.volume * sound.volume * volume;
        if(distanceControl) source.volume *= Mathf.Max(0.0f, (1.0f - Vector3.Distance(locationGetter.Invoke(), Camera.main.transform.position) / maxDistance));
    }
    bool removed = false;
    public void Stop()
    {
        if (removed) return;
        removed = true;
        AudioManager.Instance.RemoveAudio(this);
    }
}
[System.Serializable]
public struct Sound
{
    public AudioClip clip;
    public float volume;
}