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
        return new GameObject("AudioSource").AddComponent<AudioSource>();
    });
    List<PlayingAudio> playing = new(), addQueue = new(), removeQueue = new();
    public PlayingAudio PlaySound(Sound sound, Vector3 position)
    {
        if (sound.clip == null) return null;
        PlayingAudio tmp = new(sound);
        tmp.source = audioSourcePool.GetObject(position, Quaternion.identity);
        tmp.source.clip = tmp.sound.clip;
        tmp.source.volume = tmp.sound.volume;
        tmp.source.Play();
        addQueue.Add(tmp);
        return tmp;
    }
    void Update()
    {
        foreach (var i in addQueue) playing.Add(i); addQueue.Clear();
        foreach(var i in playing)
        {
            if (i.source == null)
            {
                removeQueue.Add(i);
            }
            else if (!i.source.isPlaying)
            {
                audioSourcePool.ReleaseObject(i.source);
                removeQueue.Add(i);
            }
            else if (i.locator != null) i.source.transform.position = i.locator.Invoke();
        }
        foreach (var i in removeQueue) playing.Remove(i); removeQueue.Clear();
    }
}
public static class PlayingAudioManagement
{
    public static PlayingAudio SetLocator(this PlayingAudio audio, Func<Vector3> locator)
    {
        if (audio == null) return null;
        audio.locator = locator;
        return audio;
    }
}
public class PlayingAudio
{
    public PlayingAudio(Sound sound)
    {
        this.sound = sound;
    }
    public readonly Sound sound;

    public AudioSource source;
    public Func<Vector3> locator;
}
[System.Serializable]
public struct Sound
{
    public AudioClip clip;
    public float volume;
}