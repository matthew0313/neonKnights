using DG.Tweening;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CutsceneManager_UI))]
public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }
    public CutsceneManager() => Instance = this;
    CutsceneManager_UI UIs;

    [SerializeField] BoolHook isFastPlaying;
    [SerializeField] PlayableDirector director;

    [Header("Talk")]
    [SerializeField] float talkInterval;
    [SerializeField] float talkLongInterval;

    Cutscene playing;
    private void Awake()
    {
        UIs = GetComponent<CutsceneManager_UI>();
    }

    public void StartCutscene(Cutscene cutscene)
    {
        if (playing != null) return;
        isFastPlaying.SetValue(false);
        playing = cutscene;
        foreach(var i in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICutsceneCallbackReceiver>())
        {
            i.OnCutsceneStart();
        }

        UIs.ShowEdgeUI();
        playing.Play();
    }
    public void FinishCutscene()
    {
        if (playing == null) return;

        playing.End();
        playing = null;

        Timing.KillCoroutines(playingTimeline);
        director.Stop();

        foreach (var i in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICutsceneCallbackReceiver>())
        {
            i.OnCutsceneEnd();
        }

        if (conversationOpen) CloseConversation();
        UIs.HideEdgeUI();
    }
    CoroutineHandle playingTimeline;
    public void PlayTimeline(TimelineAsset timeline, Action onFinish = null)
    {
        Timing.KillCoroutines(playingTimeline);
        director.Stop();

        playingTimeline = Timing.RunCoroutine(PlayingTimeline(timeline, onFinish));
    }
    IEnumerator<float> PlayingTimeline(TimelineAsset timeline, Action onFinish)
    {
        director.Play(timeline);
        director.playableGraph.GetRootPlayable(0).SetSpeed(isFastPlaying.value ? 10.0f : 1.0f);
        Action toggleFastPlay = () => director.playableGraph.GetRootPlayable(0).SetSpeed(isFastPlaying.value ? 10.0f : 1.0f);
        isFastPlaying.AssignCallback(toggleFastPlay);

        bool finished = false;
        Action<PlayableDirector> tmp = (PlayableDirector director) => finished = true;

        director.stopped += tmp;
        while (!finished) yield return Timing.WaitForOneFrame;
        director.stopped -= tmp;
        isFastPlaying.RemoveCallback(toggleFastPlay);

        onFinish?.Invoke();
        /*director.timeUpdateMode = DirectorUpdateMode.Manual;
        director.Play(timeline);
        director.time = director.playableAsset.duration - 0.01f;
        while (true)
        {
            yield return Timing.WaitForOneFrame;
            director.Evaluate();
            Debug.Log("Evaluated");
        }*/
    }

    CoroutineHandle conversation;
    CoroutineHandle conversationTyping;
    bool conversationOpen = false;
    public void OpenConversation(ConversationSegment segment, Action onProgress, bool autoPass = false)
    {
        if (!conversationOpen)
        {
            conversationOpen = true;
            UIs.ShowConversationUI();
        }
        Timing.KillCoroutines(conversation);
        conversation = Timing.RunCoroutine(Conversation(segment, onProgress, autoPass));
    }
    IEnumerator<float> Conversation(ConversationSegment segment, Action onProgress, bool autoPass)
    {
        UIs.SetTalkerText(segment.talker.text);
        UIs.SetTalkContentText(string.Empty);

        bool typing = true;
        conversationTyping = Timing.RunCoroutine(ConversationTyping(segment.text.text, segment.talkSound, () => typing = false));

        while (typing)
        {
            yield return Timing.WaitForOneFrame;
            if (Input.GetMouseButtonDown(0) || isFastPlaying.value)
            {
                yield return Timing.WaitForOneFrame;
                UIs.SetTalkContentText(segment.text.text);
                break;
            }
        }
        Timing.KillCoroutines(conversationTyping);
        yield return Timing.WaitForOneFrame;
        if(!autoPass) while(!Input.GetMouseButtonDown(0) && !isFastPlaying.value) yield return Timing.WaitForOneFrame;
        onProgress?.Invoke();
    }
    int SearchStringIndex(int start, string text, char search)
    {
        for(int i = start; i < text.Length; i++)
        {
            if (text[i] == search) return i;
        }
        return text.Length;
    }
    IEnumerator<float> ConversationTyping(string text, Sound talkSound, Action onFinish)
    {
        StringBuilder sb = new();
        int i = 0;
        while(i < text.Length)
        {
            if (text[i] == '<')
            {
                int tagStart = i;
                int tagEnd = SearchStringIndex(tagStart + 1, text, '>');
                if(tagEnd < text.Length)
                {
                    sb.Append(text.Substring(tagStart, tagEnd - tagStart + 1));
                    i += tagEnd - tagStart + 1;
                }
            }

            sb.Append(text[i]);
            UIs.SetTalkContentText(sb.ToString());
            AudioManager.Instance.PlaySound(talkSound);

            if (text[i] == '.' || text[i] == ',' || text[i] == '!' || text[i] == '?') yield return Timing.WaitForSeconds(talkLongInterval);
            else yield return Timing.WaitForSeconds(talkInterval);
            i++;
        }
        onFinish?.Invoke();
    }
    public void CloseConversation()
    {
        if (!conversationOpen) return;
        conversationOpen = false;
        UIs.HideConversationUI();
    }
    public void OpenConversationChoice(ConversationChoiceSegment segment, Action<int> onSelect)
    {
        if (!conversationOpen)
        {
            onSelect?.Invoke(0);
            return;
        }

        UIs.ShowChoice(segment, onSelect);
    }
    private void OnDestroy()
    {
        Timing.KillCoroutines(playingTimeline);
        Timing.KillCoroutines(conversationTyping);
        Timing.KillCoroutines(playingTimeline);
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