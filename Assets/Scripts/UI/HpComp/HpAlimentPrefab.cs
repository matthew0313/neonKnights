using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using TMPro;

public class HpAlimentPrefab : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] Image backImage;
    [SerializeField] Image alimentProgress;
    StatusAliment displayingAliment;

    public void Set(StatusAliment aliment)
    {
        displayingAliment = aliment;
        text.text = aliment.alimentDisplayString;
        backImage.color = aliment.alimentColor;
        if (aliment is IAlimentProgressDisplayed)
        {
            alimentProgress.gameObject.SetActive(true);
            AlimentProgressUpdate();
        }
        else alimentProgress.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (displayingAliment is IAlimentProgressDisplayed) AlimentProgressUpdate();
    }
    void AlimentProgressUpdate()
    {
        alimentProgress.transform.localScale = new Vector2((displayingAliment as IAlimentProgressDisplayed).progressLeft, 1.0f);
    }
}