using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class SimpleFadeout : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float duration;
    private void Start()
    {
        image.DOColor(new Color(image.color.r, image.color.g, image.color.b, 0.0f), duration);
    }
}