using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HpChangeEffect : MonoBehaviour
{
    [SerializeField] HpComp target;
    [SerializeField] Image image;

    [Header("Damage & Low Health")]
    [SerializeField] Color damageColor;
    [SerializeField] float damageIntensity;
    [SerializeField] float damageIntensityLowerTime;
    [SerializeField] float lowHealthThreshold;
    [SerializeField] float lowHealthIntensity;

    [Header("Heal")]
    [SerializeField] Color healColor;
    [SerializeField] float healIntensity;
    [SerializeField] float healIntensityLowerTime;
    private void Awake()
    {
        target.onDamage += OnDamage;
        target.onHeal += OnHeal;
        image.material = new Material(image.material);
    }
    readonly int colorID = Shader.PropertyToID("_Color");
    readonly int intensityID = Shader.PropertyToID("_VignetteIntensity");
    Tween tween = null;
    void OnDamage(DamageReceivedInfo info)
    {
        if (tween != null) tween.Kill();
        float intensity = damageIntensity;
        float targetIntensity;
        if (target.hp / target.baseHp <= lowHealthThreshold)
        {
            targetIntensity = (1.0f - target.hp / target.baseHp / lowHealthThreshold) * lowHealthIntensity;
        }
        else targetIntensity = 0.0f;

        image.material.SetColor(colorID, damageColor);
        image.material.SetFloat(intensityID, intensity);
        tween = DOTween.To(() => intensity, value => intensity = value, targetIntensity, damageIntensityLowerTime).OnUpdate(() =>
        {
            image.material.SetFloat(intensityID, intensity);
        });
    }
    void OnHeal(float healAmount)
    {
        if (tween != null) tween.Kill();
        float intensity = healIntensity;
        float targetIntensity;
        if (target.hp / target.baseHp <= lowHealthThreshold)
        {
            targetIntensity = (1.0f - target.hp / target.baseHp / lowHealthThreshold) * lowHealthIntensity;
        }
        else targetIntensity = 0.0f;

        image.material.SetColor(colorID, healColor);
        image.material.SetFloat(intensityID, intensity);
        float start = intensity;
        float counter = 0.0f;
        tween = DOTween.To(() => counter, value => counter = value, 1.0f, healIntensityLowerTime).OnUpdate(() =>
        {
            intensity = Mathf.Lerp(start, targetIntensity, counter);
            image.material.SetFloat(intensityID, intensity);
            if (targetIntensity > 0.0f) image.material.SetColor(colorID, Color.Lerp(healColor, damageColor, counter));
        });
    }
}
