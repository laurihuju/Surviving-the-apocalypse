using UnityEngine;

public class SwordController : MonoBehaviour
{
    [SerializeField] private float damage;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        HealthManager health = other.GetComponent<HealthManager>();
        if (health == null)
            return;

        health.ChangeHealth(-damage);
    }
}
