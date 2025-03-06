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
    List<ICutsceneCallbackReceiver> callbackReceivers;
    private void Awake()
    {
        UIs = GetComponent<CutsceneManager_UI>();
        callbackReceivers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICutsceneCallbackReceiver>().ToList();
        callbackReceivers.Sort((a, b) => a.executionOrder.CompareTo(b.executionOrder));
    }

    public void StartCutscene(Cutscene cutscene)
    {
        if (playing != null) return;
        isFastPlaying.SetValue(false);
        playing = cutscene;
        foreach(var i in callbackReceivers)
        {
            i.OnCutsceneStart();
        }

        UIs.ShowEdgeUI();
        playing.OnPlay();
    }
    public void FinishCutscene()
    {
        if (playing == null) return;

        playing.End();
        playing = null;

        Timing.KillCoroutines(playingTimeline);
        if (assigned) isFastPlaying.RemoveCallback(ToggleFastPlay);
        director.Stop();

        foreach (var i in callbackReceivers)
        {
            i.OnCutsceneEnd();
        }

        if (conversationOpen) CloseConversation();
        UIs.HideEdgeUI();
    }
    CoroutineHandle playingTimeline;
    public void PlayTimeline(TimelineAsset timeline, DirectorWrapMode wrapMode, Action onFinish = null)
    {
        Timing.KillCoroutines(playingTimeline);
        if (assigned) isFastPlaying.RemoveCallback(ToggleFastPlay);

        director.Stop();
        director.extrapolationMode = wrapMode;

        playingTimeline = Timing.RunCoroutine(PlayingTimeline(timeline, onFinish));
    }
    bool assigned = false;
    void ToggleFastPlay()
    {
        director.playableGraph.GetRootPlayable(0).SetSpeed(isFastPlaying.value ? 10.0f : 1.0f);
    }
    IEnumerator<float> PlayingTimeline(TimelineAsset timeline, Action onFinish)
    {
        director.Play(timeline);
        director.playableGraph.GetRootPlayable(0).SetSpeed(isFastPlaying.value ? 10.0f : 1.0f);
        isFastPlaying.AssignCallback(ToggleFastPlay);
        assigned = true;
        //bool finished = false;
        //Action<PlayableDirector> tmp = (PlayableDirector director) => finished = true;

        //director.stopped += tmp;
        //while (!stopped) yield return Timing.WaitForOneFrame;
        //director.stopped -= tmp;
        while(director.time < director.playableAsset.duration - 0.1f) yield return Timing.WaitForOneFrame;
        isFastPlaying.RemoveCallback(ToggleFastPlay);
        assigned = false;

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
        //conversationTyping = Timing.RunCoroutine(ConversationTyping(segment.text.text, segment.talkSound, () => typing = false));
        conversationTyping = Timing.RunCoroutine(CoroutineLibrary.Typewrite(segment.text.text,
            talkInterval, talkLongInterval,
            (content) =>
            {
                UIs.SetTalkContentText(content);
                AudioManager.Instance.PlaySound(segment.talkSound);
            },
            () => typing = false));

        while (typing)
        {
            yield return Timing.WaitForOneFrame;
            if ((Input.GetMouseButtonDown(0) || isFastPlaying.value) && segment.skippable)
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
    CoroutineHandle delaying;
    public void Delay(float delay, Action onFinish)
    {
        Timing.KillCoroutines(delaying);
        delaying = Timing.RunCoroutine(Delaying(delay, onFinish));
    }
    IEnumerator<float> Delaying(float delay, Action onFinish)
    {
        float counter = 0.0f;
        while(counter < delay)
        {
            yield return Timing.WaitForOneFrame;
            counter += Time.deltaTime * (isFastPlaying.value ? 10.0f : 1.0f);
        }
        onFinish?.Invoke();
    }
    /*int SearchStringIndex(int start, string text, char search)
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
    }*/
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
        if (assigned) isFastPlaying.RemoveCallback(ToggleFastPlay);
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