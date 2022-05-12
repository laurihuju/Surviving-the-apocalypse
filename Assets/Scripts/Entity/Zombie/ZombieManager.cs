using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    private static ZombieManager instance;

    [Header("Spawning")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private float maxSpawnDistanceFromPlayer;
    [SerializeField] private float minSpawnDistanceFromPlayer;
    [SerializeField] private float maxZombies;
    [SerializeField] private float minSpawnTime;
    [SerializeField] private float maxSpawnTime;

    [Header("Despawning")]
    [SerializeField] private float despawnDistance;
    [SerializeField] private float despawnCheckTime;

    [Header("Zombie drops")]
    [SerializeField] private int[] possibleDrops;
    [SerializeField] private int minDropAmount;
    [SerializeField] private int maxDropAmount;
    [SerializeField] private float maxDropDistance;

    private List<SingleZombie> zombies;
    private float nextZombieSpawn;
    private float nextDespawnCheck;

    private void Start()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        zombies = new List<SingleZombie>();
    }

    private void Update()
    {
        if (Time.time > nextDespawnCheck)
            CheckZombieDespawn();

        if (Time.time > nextZombieSpawn && zombies.Count < maxZombies)
            SpawnZombie();
    }

    public void SendSoundToZombies(float volume, Vector3 worldPosition)
    {
        for(int i = 0; i < zombies.Count; i++)
        {
            zombies[i].HearSound(volume, worldPosition);
        }
    }

    private void CheckZombieDespawn()
    {
        for (int i = 0; i < zombies.Count; i++)
        {
            if (Mathf.Abs(zombies[i].transform.position.x - PlayerController.GetInstance().transform.position.x) > despawnDistance || Mathf.Abs(zombies[i].transform.position.z - PlayerController.GetInstance().transform.position.z) > despawnDistance)
            {
                DespawnZombie(zombies[i]);
                i--;
            }
        }

        nextDespawnCheck = Time.time + despawnCheckTime;
    }

    public void DespawnZombie(SingleZombie zombie)
    {
        GroupedZombie groupedZombie = zombie.GetComponent<GroupedZombie>();
        if (groupedZombie.enabled)
            groupedZombie.GetGroup().RemoveZombie(groupedZombie);
        Destroy(zombie.gameObject);
        zombies.Remove(zombie);
    }

    private void SpawnZombie()
    {
        zombies.Add(Instantiate(zombiePrefab, GetRandomSpawnPosition(), zombiePrefab.transform.rotation).GetComponent<SingleZombie>());

        nextZombieSpawn = Time.time + Random.Range(minSpawnTime, maxSpawnTime);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float xPosition = GetRandomSpawnDistance(PlayerController.GetInstance().transform.position.x);
        float zPosition = GetRandomSpawnDistance(PlayerController.GetInstance().transform.position.z);
        float yPosition = GetSpawnHeight(xPosition, zPosition);

        return new Vector3(xPosition, yPosition, zPosition);
    }

    private float GetRandomSpawnDistance(float from)
    {
        int spawnDirection = Random.Range(0, 2);
        if (spawnDirection == 0)
            return Random.Range(from - maxSpawnDistanceFromPlayer, from - minSpawnDistanceFromPlayer);
        return Random.Range(from + minSpawnDistanceFromPlayer, from + maxSpawnDistanceFromPlayer);
    }

    private float GetSpawnHeight(float spawnX, float spawnZ)
    {
        RaycastHit hit;
        if(Physics.Raycast(new Vector3(spawnX, 100, spawnZ), Vector3.down, out hit))
        {
            return hit.point.y;
        }
        return 0;
    }

    public void SpawnRandomDrops(Vector3 deathPosition)
    {
        int dropAmount = Random.Range(minDropAmount, maxDropAmount + 1);
        for(int i = 0; i < dropAmount; i++)
        {
            int itemToDrop = Random.Range(0, possibleDrops.Length);
            GroundItemManager.GetInstance().AddGroundItem(possibleDrops[itemToDrop], 1, GetRandomItemDropPosition(deathPosition));
        }
    }

    private Vector3 GetRandomItemDropPosition(Vector3 deathPosition)
    {
        float xPosition = Random.Range(deathPosition.x - maxDropDistance, deathPosition.x + maxDropDistance);
        float zPosition = Random.Range(deathPosition.z - maxDropDistance, deathPosition.z + maxDropDistance);
        return new Vector3(xPosition, deathPosition.y, zPosition);
    }

    public SingleZombie[] GetZombiesNearPosition(Vector3 position, float maxDistance, SingleZombie zombieToNotInclude)
    {
        int zombieCount = 0;
        for(int i = 0; i < zombies.Count; i++)
        {
            if (Vector3.Distance(zombies[i].transform.position, position) > maxDistance)
                continue;
            if (zombies[i] == zombieToNotInclude)
                continue;
            zombieCount++;
        }

        if (zombieCount == 0)
            return null;

        SingleZombie[] zombiesToReturn = new SingleZombie[zombieCount];
        zombieCount = 0;
        for (int i = 0; i < zombies.Count; i++)
        {
            if (Vector3.Distance(zombies[i].transform.position, position) > maxDistance)
                continue;
            if (zombies[i] == zombieToNotInclude)
                continue;
            zombiesToReturn[zombieCount] = zombies[i];
            zombieCount++;
        }

        return zombiesToReturn;
    }

    public static ZombieManager GetInstance()
    {
        return instance;
    }
}
