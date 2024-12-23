using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField]
    Image[] staminaCharges;
    [SerializeField] float hideTime = 1.5f;
    [SerializeField] Animator hideAnim;

    int displayedCount = 0;
    Player_Movement origin;
    private void Awake()
    {
        origin = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Movement>();
        origin.onStaminaChange += OnStaminaChange;
    }
    float counter = 0.0f;
    readonly int showID = Animator.StringToHash("Show");
    void OnStaminaChange()
    {
        hideAnim.SetBool(showID, true);
        counter = 0.0f;
        if (displayedCount < Mathf.CeilToInt(origin.maxStamina))
        {
            for(;displayedCount < Mathf.CeilToInt(origin.maxStamina);)
            {
                if (displayedCount >= staminaCharges.Length) break;
                staminaCharges[displayedCount++].gameObject.SetActive(true);
            }
        }
        else if(displayedCount > Mathf.CeilToInt(origin.maxStamina))
        {
            for (; displayedCount > Mathf.CeilToInt(origin.maxStamina);)
            {
                staminaCharges[--displayedCount].gameObject.SetActive(false);
            }
        }
        for(int i = 0; i < displayedCount; i++)
        {
            float scale = Mathf.Clamp(origin.stamina - i, 0.0f, 1.0f);
            staminaCharges[i].transform.localScale = new Vector2(0.2f + 0.8f * scale, 0.2f + 0.8f * scale);
            float hue;
            Color.RGBToHSV(staminaCharges[i].color, out hue, out _, out _);
            staminaCharges[i].color = Color.HSVToRGB(hue, (scale == 0.0f ? 1.0f : scale), (scale == 0.0f ? 0.5f : 1.0f));
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
