using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PauseUI : MonoBehaviour
{
    [SerializeField] RectTransform buttonRack;
    [SerializeField] RectTransform[] buttons;
    Tween buttonRackTween;
    Tween[] buttonTweens;
    private void OnEnable()
    {
        if(buttonRackTween == null)
        {
            buttonRackTween = buttonRack.DOAnchorPosX(0.0f, 1.0f).SetAutoKill(false).SetUpdate(true);
        }
        else
        {
            buttonRackTween.Restart(true);
        }
        if (buttonTweens == null) buttonTweens = new Tween[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttonTweens[i] == null)
            {
                buttonTweens[i] = buttons[i].DOAnchorPosX(0.0f, 1.0f).SetEase(Ease.OutBack).SetDelay(0.5f + 0.1f * i).SetAutoKill(false).SetUpdate(true);
            }
            else
            {
                buttonTweens[i].Restart(true);
            }
        }
    }
}
