using UnityEngine;

public class FireLight : MonoBehaviour
{
    [SerializeField] private Light fireLight;

    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float changeSpeed;
    [SerializeField] private float minGenerateWait;
    [SerializeField] private float maxGenerateWait;

    private float targetIntensity = 0;
    private float nextGenerateTime = 0;

    void FixedUpdate()
    {
        if (Time.time > nextGenerateTime)
            GenerateTargetIntensity();

        fireLight.intensity = Mathf.Lerp(fireLight.intensity, targetIntensity, changeSpeed);
    }

    private void GenerateTargetIntensity()
    {
        nextGenerateTime = Time.time + Random.Range(maxGenerateWait, maxGenerateWait);

        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }
}
