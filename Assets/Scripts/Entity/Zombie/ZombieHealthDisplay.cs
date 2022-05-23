using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZombieHealthDisplay : MonoBehaviour, HealthDisplay
{
    [Header("Settings")]
    [SerializeField] private float barSmoothness = 0.3f;
    [SerializeField] private Gradient healthBarColor;

    [Header("Health UI")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image healthBarFill;

    [Header("Health Manager")]
    [SerializeField] private HealthManager healthManager;

    private float targetPercentage = 1;

    private void Update()
    {
        healthSlider.value = Mathf.Lerp(healthSlider.value, targetPercentage, barSmoothness);

        healthBarFill.color = healthBarColor.Evaluate(healthSlider.value);

        Vector3 lookPosition = PlayerController.GetInstance().transform.position;
        lookPosition.y = healthSlider.transform.position.y;
        healthSlider.transform.rotation = Quaternion.LookRotation(healthSlider.transform.position - lookPosition);
    }

    public void SetPercentage(float percentage)
    {
        targetPercentage = percentage;

        healthText.text = healthManager.GetHealth() + " / " + healthManager.GetMaxHealth();
    }
}
