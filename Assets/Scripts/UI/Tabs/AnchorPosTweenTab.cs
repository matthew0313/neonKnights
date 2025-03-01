using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AnchorPosTweenTab : Tab
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Vector2 destination;
    [SerializeField] float duration;
    [SerializeField] Ease ease;
    [SerializeField] bool independantUpdate = true;
    Tween tween;
    public override void OpenTab()
    {
        base.OpenTab();
        if (tween == null)
        {
            tween = rectTransform.DOAnchorPos(destination, duration).SetEase(ease).SetAutoKill(false).SetUpdate(independantUpdate);
        }
        else
        {
            tween.OnStepComplete(null);
            tween.Restart();
        }
    }
    public override void CloseTab()
    {
        tween.OnStepComplete(() => base.CloseTab());
        tween.PlayBackwards();
    }
    private void OnDisable()
    {
        tween.Rewind();
    }
}