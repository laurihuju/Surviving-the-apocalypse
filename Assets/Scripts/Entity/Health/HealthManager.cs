using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Tooltip("The maximum health amount that can be set to the health manager.")]
    [SerializeField] private float maxHealth;
    [Tooltip("The default amount of health.")]
    [SerializeField] private float health;

    private HealthDisplay healthDisplay;
    private EntityDeath death;
    private DamageEvent[] damageEvents;

    private void Start()
    {
        healthDisplay = GetComponent<HealthDisplay>();
        death = GetComponent<EntityDeath>();

        damageEvents = GetComponents<DamageEvent>();
    }

    /// <summary>
    /// Sets the health amount of the health manager.
    /// </summary>
    /// <param name="health"></param>
    public void SetHealth(float health)
    {
        this.health = health;

        if (this.health > maxHealth)
            this.health = maxHealth;
        else if (this.health < 0)
            this.health = 0;

        if (this.health == 0)
            if (death != null)
                death.OnDeath();

        UpdateHealthDisplay();
    }

    /// <summary>
    /// Changes the health of the health manager by given amount. Amount can be positive or negative.
    /// </summary>
    /// <param name="changeAmount"></param>
    public void ChangeHealth(float changeAmount)
    {
        SetHealth(health + changeAmount);
        if(!CompareTag("Player"))
        Debug.Log(this.health);

        if (changeAmount < 0)
            if (damageEvents != null)
                for (int i = 0; i < damageEvents.Length; i++)
                    damageEvents[i].OnDamage();
    }

    /// <summary>
    /// Sets the max health of the health manager.
    /// </summary>
    /// <param name="maxHealth"></param>
    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;

        if (health > maxHealth)
            health = maxHealth;

        UpdateHealthDisplay();
    }

    /// <summary>
    /// Returns the health amount of the health manager.
    /// </summary>
    /// <returns></returns>
    public float GetHealth()
    {
        return health;
    }

    /// <summary>
    /// Return the max health of the health manager.
    /// </summary>
    /// <returns></returns>
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// Update the health display if it exists.
    /// </summary>
    private void UpdateHealthDisplay()
    {
        if (healthDisplay == null)
            return;
        healthDisplay.SetPercentage(health / maxHealth);
    }
}
