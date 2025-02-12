using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager_UI : MonoBehaviour
{
    [Header("UIs")]
    [SerializeField] float edgeMoveTime = 0.5f;
    [SerializeField] RectTransform upEdge, downEdge;

    [SerializeField] float conversationUIAppearTime = 0.25f;
    [SerializeField] float conversationUIAppearPivot = -0.35f;
    [SerializeField] RectTransform conversationUI;
    [SerializeField] TMP_Text talkerText, talkContentText;

    [SerializeField] Transform conversationChoiceAnchor;
    [SerializeField] ConversationChoiceUI conversationChoicePrefab;
    List<ConversationChoiceUI> conversationChoices = new();

    Tween edgeUITween;
    float edgeUIAppear = 0.0f;
    public void ShowEdgeUI()
    {
        if (edgeUITween == null)
        {
            edgeUITween = DOTween.To(() => edgeUIAppear, value => edgeUIAppear = value, 1.0f, edgeMoveTime).OnUpdate(() =>
            {
                upEdge.pivot = new Vector2(0.5f, edgeUIAppear);
                upEdge.anchoredPosition = Vector2.zero;

                downEdge.pivot = new Vector2(0.5f, 1.0f - edgeUIAppear);
                downEdge.anchoredPosition = Vector2.zero;
            }).SetEase(Ease.OutCirc).SetAutoKill(false);
        }
        else
        {
            edgeUITween.SetEase(Ease.OutCirc).PlayForward();
        }
    }
    public void HideEdgeUI()
    {
        if (edgeUITween == null) return;
        edgeUITween.SetEase(Ease.InCirc).PlayBackwards();
    }

    Tween conversationUITween;
    float conversationUIAppear = 0.0f;
    public void ShowConversationUI()
    {
        if(conversationUITween == null)
        {
            conversationUITween = DOTween.To(() => conversationUIAppear, value => conversationUIAppear = value, 1.0f, conversationUIAppearTime).OnUpdate(() =>
            {
                conversationUI.pivot = new Vector2(0.5f, Mathf.Lerp(1.0f, conversationUIAppearPivot, conversationUIAppear));
                conversationUI.anchoredPosition = Vector2.zero;
            }).SetEase(Ease.OutCirc).SetAutoKill(false);
        }
        else
        {
            conversationUITween.SetEase(Ease.OutCirc).PlayForward();
        }
    }
    public void HideConversationUI()
    {
        if (conversationUITween == null) return;
        conversationUITween.SetEase(Ease.InCirc).PlayBackwards();
    }
    public void SetTalkerText(string talker)
    {
        talkerText.text = talker;
    }
    public void SetTalkContentText(string talkContent)
    {
        talkContentText.text = talkContent;
    }
    public void ShowChoice(ConversationChoiceSegment segment, Action<int> onSelect)
    {
        for(int i = 0; i < segment.options.Length; i++)
        {
            if (conversationChoices.Count <= i) conversationChoices.Add(Instantiate(conversationChoicePrefab, conversationChoiceAnchor));

            conversationChoices[i].Show();
            conversationChoices[i].text.text = segment.options[i];

            int optionIndex = i;
            conversationChoices[i].onClick = () =>
            {
                onSelect?.Invoke(optionIndex);
                HideChoice();
            };
        }
    }
    void HideChoice()
    {
        for(int i = 0; i < conversationChoices.Count; i++)
        {
            if (conversationChoices[i].shown) conversationChoices[i].Hide();
        }
    }
}