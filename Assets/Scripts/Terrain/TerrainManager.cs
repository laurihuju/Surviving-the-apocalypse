using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [SerializeField] private Terrain[] terrains;

    private void Start()
    {
        for(int i = 0; i < terrains.Length; i++)
        {
            TerrainData clonedData = TerrainDataCloner.Clone(terrains[i].terrainData);
            terrains[i].terrainData = clonedData;
            terrains[i].GetComponent<TerrainCollider>().terrainData = clonedData;
        }
    }
}