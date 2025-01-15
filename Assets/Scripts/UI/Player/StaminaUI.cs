using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] StaminaCharge staminaChargePrefab;
    [SerializeField] Transform staminaChargeAnchor;
    [SerializeField] float hideTime = 1.5f;
    [SerializeField] Animator hideAnim;
    List<StaminaCharge> staminaCharges = new();
    Player_Movement origin;
    private void Awake()
    {
        origin = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Movement>();
        origin.onStaminaChange += OnStaminaChange;
    }
    float counter = 0.0f;
    bool showing = false;
    readonly int showID = Animator.StringToHash("Show");
    void OnStaminaChange()
    {
        hideAnim.SetBool(showID, true);
        counter = 0.0f;
        int displayCount = Mathf.CeilToInt(origin.maxStamina);
        int i;
        for(i = 0; i < displayCount; i++)
        {
            if(staminaCharges.Count <= i)
            {
                staminaCharges.Add(Instantiate(staminaChargePrefab, staminaChargeAnchor));
            }
            staminaCharges[i].gameObject.SetActive(true);
        }
        for(; i < staminaCharges.Count; i++)
        {
            staminaCharges[i].gameObject.SetActive(false);
        }
        for(i = 0; i < displayCount; i++)
        {
            float scale = Mathf.Clamp(origin.stamina - i, 0.0f, 1.0f);
            staminaCharges[i].SetScale(0.2f + 0.8f * scale);
            float hue;
            Color.RGBToHSV(staminaCharges[i].image.color, out hue, out _, out _);
            staminaCharges[i].image.color = Color.HSVToRGB(hue, (scale == 0.0f ? 1.0f : scale), (scale == 0.0f ? 0.5f : 1.0f));
        }
    }
    private void Update()
    {
        if(origin.stamina >= origin.maxStamina)
        {
            if (counter < hideTime)
            {
                counter += Time.deltaTime;
                if (counter >= hideTime) hideAnim.SetBool(showID, false);
            }
        }
    }
    private void OnDestroy()
    {
        origin.onStaminaChange -= OnStaminaChange;
    }
}
