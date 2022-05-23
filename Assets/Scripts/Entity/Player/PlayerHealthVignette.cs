using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealthVignette : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Volume volume;

    [Header("Vignette")]
    [SerializeField] private Gradient vignetteColor;
    [SerializeField] private AnimationCurve vignetteIntensity;
    [SerializeField] private AnimationCurve vignetteSmoothness;
    [SerializeField] [Range(0, 0.1f)] private float vignetteChangeSmoothness;
    private Vignette vignette;

    [Header("Health Manager")]
    [SerializeField] private HealthManager healthManager;

    private void Start()
    {
        volume.profile.TryGet(out vignette);
    }

    private void FixedUpdate()
    {
        vignette.color.value = Color.Lerp(vignette.color.value, vignetteColor.Evaluate(healthManager.GetHealth() / healthManager.GetMaxHealth()), vignetteChangeSmoothness);
        vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, vignetteIntensity.Evaluate(healthManager.GetHealth() / healthManager.GetMaxHealth()), vignetteChangeSmoothness);
        vignette.smoothness.value = Mathf.Lerp(vignette.smoothness.value, vignetteSmoothness.Evaluate(healthManager.GetHealth() / healthManager.GetMaxHealth()), vignetteChangeSmoothness);
    }
}
