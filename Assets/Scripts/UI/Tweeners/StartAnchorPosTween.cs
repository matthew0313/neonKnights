using UnityEngine;
using TMPro;
using DG.Tweening;

public class StartAnchorPosTween : MonoBehaviour
{
    [SerializeField] RectTransform tweened;
    [SerializeField] Vector2 targetPos;
    [SerializeField] float duration = 1.0f, delay = 0.0f;
    [SerializeField] Ease easeType;
    private void Start()
    {
        tweened.DOAnchorPos(targetPos, duration).SetEase(easeType).SetDelay(delay);
    }
}