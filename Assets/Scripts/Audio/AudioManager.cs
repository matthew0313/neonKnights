using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;
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
        AudioSource tmp = new GameObject("PlayingAudio").AddComponent<AudioSource>();
        tmp.playOnAwake = false;
        return tmp;
    }, 999999, 0);
    List<PlayingAudio> playing = new(), addQueue = new(), removeQueue = new();
    public PlayingAudio PlaySound(Sound sound)
    {
        if (sound.clip == null) return null;
        PlayingAudio tmp = new(sound, audioSourcePool.GetObject());
        tmp.source.time = 0.0f;
        addQueue.Add(tmp);
        return tmp;
    }
    private void Awake()
    {
        GlobalManager.Instance.onSceneWillLoad += OnSceneWillLoad;
    }
    void OnSceneWillLoad()
    {
        playing.Clear();
    }
    void Update()
    {
        foreach(var i in playing)
        {
            i.Update();
        }
        foreach (var i in addQueue)
        {
            i.source.Play();
            playing.Add(i);
        }
        addQueue.Clear();
        foreach (var i in removeQueue)
        {
            audioSourcePool.ReleaseObject(i.source);
            playing.Remove(i);
        }
        removeQueue.Clear();
    }
    public void StopAudio(PlayingAudio audio)
    {
        removeQueue.Add(audio);
    }
    public void StopAudio(string id)
    {
        if (id == string.Empty) return;
        playing.Find(audio => audio.id == id)?.Stop();
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
        audio.lastLocation = locationGetter.Invoke();
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
        return audio;
    }
    public static PlayingAudio SetID(this PlayingAudio audio, string id)
    {
        if (audio == null) return null;
        audio.id = id;
        return audio;
    }
}
public class PlayingAudio
{
    public PlayingAudio(Sound sound, AudioSource source)
    {
        this.sound = sound;
        this.source = source;
        source.time = 0;
        source.loop = false;
        source.clip = sound.clip;
    }
    public readonly Sound sound;
    public readonly AudioSource source;

    public float volume = 1.0f;

    public bool distanceControl = false;
    public Func<Vector3> locationGetter;
    public Vector3 lastLocation;
    public float maxDistance = 30;

    public string id = string.Empty;
    public void Update()
    {
        if (!source.isPlaying)
        {
            Stop();
            return;
        }
        source.volume = Settings.volume * sound.volume * volume;
        if (distanceControl)
        {
            Vector3 pos;
            try
            {
                pos = locationGetter.Invoke();
            }
            catch
            {
                pos = lastLocation;
            }
            source.volume *= Mathf.Max(0.0f, (1.0f - Vector3.Distance(pos, Camera.main.transform.position) / maxDistance));
            lastLocation = pos;
        }
    }
    bool removed = false;
    public void Stop()
    {
        if (removed) return;
        removed = true;
        AudioManager.Instance.StopAudio(this);
    }
}
[System.Serializable]
public struct Sound
{
    public AudioClip clip;
    public float volume;
}