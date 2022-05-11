using UnityEngine;

public class ZombieDeath : MonoBehaviour, EntityDeath
{
    public void OnDeath()
    {
        ZombieManager.GetInstance().SpawnRandomDrops(transform.position);

        ZombieManager.GetInstance().DespawnZombie(GetComponent<SingleZombie>());
    }
}
