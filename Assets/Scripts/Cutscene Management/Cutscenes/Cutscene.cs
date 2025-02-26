using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Unity.Cinemachine;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(Animator))]
public class Cutscene : MonoBehaviour
{
    Animator anim;
    readonly int playID = Animator.StringToHash("Play");

    [SerializeField] CinemachineCamera[] cinemachineCameras;
    [SerializeField] UnityEvent[] events;
    [SerializeField] UnityEvent onCutsceneFinish;
    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        foreach (var i in anim.GetBehaviours<CameraPriorityBehaviour>()) i.setPriority += SetPriority;
        foreach (var i in anim.GetBehaviours<EvokeEventBehaviour>()) i.invokeEvent += InvokeEvent;
    }
    void SetPriority(int cameraIndex, int priority)
    {
        if (cameraIndex < 0 || cameraIndex >= cinemachineCameras.Length)
        {
            Debug.LogWarning("Invalid camera index");
            return;
        }

        cinemachineCameras[cameraIndex].Priority = priority;
    }
    void InvokeEvent(int eventIndex)
    {
        if(eventIndex < 0 || eventIndex >= events.Length)
        {
            Debug.LogWarning("Invalid event index");
            return;
        }
        events[eventIndex].Invoke();
    }
    public void Play()
    {
        anim.SetTrigger(playID);
    }
    public virtual void End()
    {
        onCutsceneFinish?.Invoke();
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Cutscene))]
public class Cutscene_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Start Cutscene")) CutsceneManager.Instance.StartCutscene(target as Cutscene);
    }
}
#endif