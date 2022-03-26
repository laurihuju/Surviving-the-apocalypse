using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    [Header("Stamina")]
    [Tooltip("The maximum amount of stamina that player can have.")]
    [SerializeField] private float maxStamina;
    [Tooltip("The default amount of stamina that player has on the game start.")]
    [SerializeField] private float stamina;

    [Header("Hunger")]
    [Tooltip("The maximum amount of saturation that player can have.")]
    [SerializeField] private float maxSaturation;
    [Tooltip("The default amount of saturation that player has on the game start.")]
    [SerializeField] private float saturation;

    public void SetStamina(float stamina)
    {
        this.stamina = stamina;

        if (this.stamina > maxStamina)
            this.stamina = maxStamina;
        else if (this.stamina < 0)
            this.stamina = 0;

        UpdateStaminaDisplay();
    }

    public void SetSaturation(float saturation)
    {
        this.saturation = saturation;

        if (this.saturation > maxSaturation)
            this.saturation = maxSaturation;
        else if (this.saturation < 0)
            this.saturation = 0;

        UpdateHungerDisplay();
    }

    public void ChangeStamina(float changeAmount)
    {
        SetStamina(stamina + changeAmount);
    }

    public void ChangeSaturation(float changeAmount)
    {
        SetSaturation(saturation + changeAmount);
    }

    public void SetMaxStamina(float maxStamina)
    {
        this.maxStamina = maxStamina;

        if (stamina > maxStamina)
            stamina = maxStamina;

        UpdateStaminaDisplay();
    }

    public void SetMaxSaturation(float maxSaturation)
    {
        this.maxSaturation = maxSaturation;

        if (saturation > maxSaturation)
            saturation = maxSaturation;

        UpdateHungerDisplay();
    }

    private void UpdateStaminaDisplay()
    {

    }

    private void UpdateHungerDisplay()
    {

    }
}
