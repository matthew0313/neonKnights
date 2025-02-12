using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(Animator))]
public class Cutscene : MonoBehaviour
{
    Animator anim;
    readonly int playID = Animator.StringToHash("Play");
    private void Awake()
    {
        anim = GetComponent<Animator>();
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