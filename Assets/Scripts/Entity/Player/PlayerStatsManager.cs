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

    /// <summary>
    /// Sets the player's stamina.
    /// </summary>
    /// <param name="stamina"></param>
    public void SetStamina(float stamina)
    {
        this.stamina = stamina;

        if (this.stamina > maxStamina)
            this.stamina = maxStamina;
        else if (this.stamina < 0)
            this.stamina = 0;

        UpdateStaminaDisplay();
    }

    /// <summary>
    /// Sets the player's saturation.
    /// </summary>
    /// <param name="saturation"></param>
    public void SetSaturation(float saturation)
    {
        this.saturation = saturation;

        if (this.saturation > maxSaturation)
            this.saturation = maxSaturation;
        else if (this.saturation < 0)
            this.saturation = 0;

        UpdateHungerDisplay();
    }

    /// <summary>
    /// Changes the player's stamina amount by given parameter changeAmount. ChangeAmount can be positive or negative.
    /// </summary>
    /// <param name="changeAmount"></param>
    public void ChangeStamina(float changeAmount)
    {
        SetStamina(stamina + changeAmount);
    }

    /// <summary>
    /// Changes the player's saturation amount by given parameter changeAmount. ChangeAmount can be positive or negative.
    /// </summary>
    /// <param name="changeAmount"></param>
    public void ChangeSaturation(float changeAmount)
    {
        SetSaturation(saturation + changeAmount);
    }

    /// <summary>
    /// Sets the max stamina amount of the player.
    /// </summary>
    /// <param name="maxStamina"></param>
    public void SetMaxStamina(float maxStamina)
    {
        this.maxStamina = maxStamina;

        if (stamina > maxStamina)
            stamina = maxStamina;

        UpdateStaminaDisplay();
    }

    /// <summary>
    /// Sets the max saturation of the player.
    /// </summary>
    /// <param name="maxSaturation"></param>
    public void SetMaxSaturation(float maxSaturation)
    {
        this.maxSaturation = maxSaturation;

        if (saturation > maxSaturation)
            saturation = maxSaturation;

        UpdateHungerDisplay();
    }

    /// <summary>
    /// Returns the stamina of the player.
    /// </summary>
    /// <returns></returns>
    public float GetStamina()
    {
        return stamina;
    }

    /// <summary>
    /// Return the saturation of the player.
    /// </summary>
    /// <returns></returns>
    public float GetSaturation()
    {
        return saturation;
    }

    private void UpdateStaminaDisplay()
    {

    }

    private void UpdateHungerDisplay()
    {

    }
}
