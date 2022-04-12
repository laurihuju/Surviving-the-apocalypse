using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    private static ZombieManager instance;

    [Header("Zombie drops")]
    [SerializeField] private int[] possibleDrops;
    [SerializeField] private int minDropAmount;
    [SerializeField] private int maxDropAmount;
    [SerializeField] private float maxDropDistance;

    private void Start()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
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

    public static ZombieManager GetInstance()
    {
        return instance;
    }
}
