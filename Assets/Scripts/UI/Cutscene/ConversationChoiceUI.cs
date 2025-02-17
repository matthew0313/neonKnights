using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationChoiceUI : MonoBehaviour
{
    [SerializeField] Button m_button;
    [SerializeField] TMP_Text m_text;
    [SerializeField] float showTime = 0.25f;
    public Button button => m_button;
    public TMP_Text text => m_text;

    Tween tween;
    float openMeter = 0.0f;
    public bool shown { get; private set; } = false;
    public void Show()
    {
        if (shown) return;
        shown = true;

        gameObject.SetActive(true);
        if (tween == null)
        {
            tween = DOTween.To(() => openMeter, value => openMeter = value, 1.0f, showTime).OnUpdate(() =>
            {
                Color tmp = button.image.color;
                button.image.color = new Color(tmp.r, tmp.g, tmp.b, openMeter);

                tmp = text.color;
                text.color = new Color(tmp.r, tmp.g, tmp.b, openMeter);
            }).SetAutoKill(false);
        }
        tween.OnComplete(null);
        tween.PlayForward();
    }
    public void Hide()
    {
        if (!shown) return;
        shown = false;

        if (tween == null) return;
        tween.OnComplete(() => { gameObject.SetActive(false); });
        tween.PlayBackwards();
    }
    public Action onClick;
    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }
    void OnClick()
    {
        onClick?.Invoke();
    }
}
