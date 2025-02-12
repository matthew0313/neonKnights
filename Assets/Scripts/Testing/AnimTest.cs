using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class AnimTest : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController controller;
    [SerializeField] AnimationClip clip;
    [SerializeField]
    [Range(0.0f, 1.0f)] float clipWeight = 0.0f;
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        PlayableGraph graph = PlayableGraph.Create();

        AnimationPlayableOutput animOutput = AnimationPlayableOutput.Create(graph, "AnimationOutput", anim);
        AnimationMixerPlayable topMixer = AnimationMixerPlayable.Create(graph, 2);
        animOutput.SetSourcePlayable(topMixer);

        AnimatorControllerPlayable controllerPlayable = AnimatorControllerPlayable.Create(graph, controller);
        AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(graph, clip);
    }
}
