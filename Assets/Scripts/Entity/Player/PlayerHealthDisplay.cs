using UnityEngine;
using TMPro;

public class PlayerHealthDisplay : MonoBehaviour, HealthDisplay
{
    [SerializeField] private TextMeshProUGUI healthText;

    public void SetPercentage(float percentage)
    {
        healthText.text = "Health: " + percentage * 100 + "%";
    }
}
