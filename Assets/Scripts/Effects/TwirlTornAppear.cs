using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwirlTornAppear : MonoBehaviour
{
    [SerializeField] MeshRenderer[] renderers;
    [SerializeField] float startIntensity = 0.75f, appearTime = 0.5f;
    float counter = 0.0f;
    readonly int shredIntensityID = Shader.PropertyToID("_Shred_Intensity");
    private void OnEnable()
    {
        counter = 0.0f;
        foreach(var i in renderers)
        {
            i.material.SetFloat(shredIntensityID, startIntensity);
        }
    }
    void Update()
    {
        if(counter < appearTime)
        {
            counter += Time.deltaTime;
            foreach (var i in renderers)
            {
                i.material.SetFloat(shredIntensityID, Mathf.Lerp(startIntensity, 0.0f, counter / appearTime));
            }
        }
    }
}
