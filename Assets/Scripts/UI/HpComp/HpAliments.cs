using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

public class HpAliments : MonoBehaviour
{
    [SerializeField] HpComp target;
    [SerializeField] Transform prefabAnchor;
    [SerializeField] HpAlimentPrefab prefab;
    List<HpAlimentPrefab> prefabs = new();
    private void OnEnable()
    {
        target.onAlimentUpdate += OnAlimentUpdate;
        OnAlimentUpdate();
    }
    private void OnDisable()
    {
        target.onAlimentUpdate -= OnAlimentUpdate;
    }
    void OnAlimentUpdate()
    {
        int i = 0;
        foreach(var aliment in target.GetAliments())
        {
            if(prefabs.Count <= i) prefabs.Add(MonoBehaviour.Instantiate(prefab, prefabAnchor));
            prefabs[i].gameObject.SetActive(true);
            prefabs[i++].Set(aliment);
        }
        for(; i < prefabs.Count; i++)
        {
            prefabs[i].gameObject.SetActive(false);
        }
    }
}