using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class EndCreditRoll : MonoBehaviour
{
    [SerializeField] RectTransform rolled;
    [SerializeField] float startDelay = 3.0f;
    [SerializeField] float rollSpeed = 100.0f;
    CoroutineHandle coroutine;
    private void Start()
    {
        coroutine = Timing.RunCoroutine(CreditRoll());
    }
    private void OnDestroy()
    {
        Timing.KillCoroutines(coroutine);
    }
    IEnumerator<float> CreditRoll()
    {
        yield return Timing.WaitForSeconds(startDelay);

        while (true)
        {
            yield return Timing.WaitForOneFrame;
            rolled.anchoredPosition += Vector2.up * rollSpeed * Time.deltaTime;
        }
    }
}