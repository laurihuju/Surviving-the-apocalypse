using UnityEngine;

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

    /*[Header("Clouds")]
    [SerializeField] private Renderer cloudRenderer;
    [SerializeField] private Gradient cloudColor;
    [SerializeField] private Color testi;*/

    [Header("Other")]
    [SerializeField] private AnimationCurve ambientIntensity;
    [SerializeField] private Gradient fogColor;

    private void FixedUpdate()
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

        RenderSettings.fogColor = fogColor.Evaluate(time);

        /*cloudRenderer.sharedMaterial.EnableKeyword("CloudColor");
        cloudRenderer.sharedMaterial.SetColor("CloudColor", testi);
        Debug.Log(cloudRenderer.sharedMaterial.GetColor("CloudColor"));*/
    }
}
