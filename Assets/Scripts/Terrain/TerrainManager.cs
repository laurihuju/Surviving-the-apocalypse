using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    private static TerrainManager instance;

    [SerializeField] private Terrain[] terrains;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        for(int i = 0; i < terrains.Length; i++)
        {
            TerrainData clonedData = TerrainDataCloner.Clone(terrains[i].terrainData);
            terrains[i].terrainData = clonedData;
            terrains[i].GetComponent<TerrainCollider>().terrainData = clonedData;
        }
    }

    public Terrain GetTerrainInPosition(Vector3 position)
    {
        for(int i = 0; i < terrains.Length; i++)
        {
            Terrain terrain = terrains[i];
            if (position.x >= terrain.transform.position.x && position.z >= terrain.transform.position.z && position.x < terrain.transform.position.x + terrain.terrainData.size.x && position.z < terrain.transform.position.z + terrain.terrainData.size.z)
                return terrain;
        }
        return null;
    }

    public static TerrainManager GetInstance()
    {
        return instance;
    }
}