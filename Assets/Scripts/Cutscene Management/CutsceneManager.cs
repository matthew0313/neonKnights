using DG.Tweening;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CutsceneManager_UI))]
public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }
    public CutsceneManager() => Instance = this;
    CutsceneManager_UI UIs;

    [SerializeField] PlayableDirector director;
    Cutscene playing;
    private void Awake()
    {
        UIs = GetComponent<CutsceneManager_UI>();
    }

    public void StartCutscene(Cutscene cutscene)
    {
        if (playing != null) return;
        playing = cutscene;
        foreach(var i in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICutsceneCallbackReceiver>())
        {
            i.OnCutsceneStart();
        }

        UIs.ShowEdgeUI();
    }
    public void FinishCutscene()
    {
        if (playing == null) return;
        playing = null;
        foreach (var i in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICutsceneCallbackReceiver>())
        {
            i.OnCutsceneEnd();
        }

        UIs.HideEdgeUI();
    }
    CoroutineHandle playingTimeline;
    public void PlayTimeline(TimelineAsset timeline, Action onFinish = null)
    {
        Timing.KillCoroutines(playingTimeline);
        playingTimeline = Timing.RunCoroutine(PlayingTimeline(timeline, onFinish));
    }
    IEnumerator<float> PlayingTimeline(TimelineAsset timeline, Action onFinish)
    {
        director.Play(timeline);
        while (director.state == PlayState.Playing) yield return Timing.WaitForOneFrame;
        onFinish?.Invoke();
    }

    bool conversationOpen = false;
    public void OpenConversation(ConversationSegment segment)
    {
        if (!conversationOpen)
        {
            conversationOpen = true;
        }
    }
    public void CloseConversation()
    {

    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(CutsceneManager))]
public class CutsceneManager_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("End Cutscene")) (target as CutsceneManager).FinishCutscene();
    }
}
#endif