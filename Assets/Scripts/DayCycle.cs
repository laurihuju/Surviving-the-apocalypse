using UnityEngine;
using UnityEngine.Rendering.Universal;
using OD;

public class DayCycle : MonoBehaviour
{
    [Header("Time settings")]
    [SerializeField] [Range(0, 1)] private float time;
    [SerializeField] private float timeAddition;

    [Header("Sun")]
    [SerializeField] private Light sun;
    [SerializeField] private Gradient sunColor;
    [SerializeField] private AnimationCurve sunIntensity;

    [Header("Moon")]
    [SerializeField] private Light moon;
    [SerializeField] private Gradient moonColor;
    [SerializeField] private AnimationCurve moonIntensity;

    [Header("Fog")]
    [SerializeField] private Gradient fogColor;
    [SerializeField] private Gradient fogSunColor;
    [SerializeField] private AtmosphericFogRenderFeature fogFeature;

    [Header("Other")]
    [SerializeField] private AnimationCurve ambientIntensity;

    private void LateUpdate()
    {
        time += timeAddition * Time.deltaTime;

        if (time > 1)
            time -= 1;

        sun.transform.rotation = Quaternion.Euler(360 * time, sun.transform.rotation.y, sun.transform.rotation.z);
        moon.transform.rotation = Quaternion.Inverse(sun.transform.rotation);

        sun.color = sunColor.Evaluate(time);
        moon.color = moonColor.Evaluate(time);

        sun.intensity = sunIntensity.Evaluate(time);
        moon.intensity = moonIntensity.Evaluate(time);

        RenderSettings.ambientIntensity = ambientIntensity.Evaluate(time);

        fogFeature.settings.color = fogColor.Evaluate(time);
        fogFeature.settings.sunColor = fogSunColor.Evaluate(time);
    }
}
