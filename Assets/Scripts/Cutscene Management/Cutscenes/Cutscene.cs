using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Unity.Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(Animator))]
public class Cutscene : MonoBehaviour
{
    Animator anim;
    readonly int playID = Animator.StringToHash("Play");

    [SerializeField] CinemachineCamera[] cinemachineCameras;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        foreach (var i in anim.GetBehaviours<CameraPriorityBehaviour>()) i.setPriority += SetPriority;
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
    public virtual void Play()
    {
        anim.SetTrigger(playID);
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