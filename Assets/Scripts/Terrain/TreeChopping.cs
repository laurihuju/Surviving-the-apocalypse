using UnityEngine;
using System.Collections;
using EzySlice;

public class TreeChopping : MonoBehaviour
{
    private static TreeChopping instance;

    //Debug
    public Terrain terrain;
    public Transform testChopPoint;

    [SerializeField] private float sliceHeight;

    [SerializeField] private Material treeMaterial;

    private void Start()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        StartCoroutine(Testi());
    }

    private IEnumerator Testi()
    {
        yield return new WaitForSeconds(5);
        ChopTree(terrain, testChopPoint.position);
    }

    public void ChopTree(Terrain terrain, Vector3 position)
    {
        if (position.y < terrain.SampleHeight(position) + 0.2f)
            return;

        TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
        for(int i = 0; i < treeInstances.Length; i++)
        {
            Vector3 treePosition = Vector3.Scale(treeInstances[i].position, terrain.terrainData.size) + terrain.transform.position;
            if (Mathf.Abs(treePosition.x - position.x) > 1)
                continue;
            if (Mathf.Abs(treePosition.z - position.z) > 1)
                continue;

            GameObject treePrefab = terrain.terrainData.treePrototypes[treeInstances[i].prototypeIndex].prefab;
            CapsuleCollider treeCollider = treePrefab.GetComponent<CapsuleCollider>();
            if (treeCollider == null)
                continue;
            SlicedHull slicedTree = treePrefab.Slice(treePosition + Vector3.up * sliceHeight, Vector3.up);

            slicedTree.CreateLowerHull(treePrefab, treeMaterial).AddComponent<CapsuleCollider>();

            GameObject upperTree = slicedTree.CreateUpperHull(treePrefab, treeMaterial);
            upperTree.AddComponent<Rigidbody>().AddForce(Vector3.Normalize(treePosition - PlayerController.GetInstance().transform.position), ForceMode.Impulse);
            CapsuleCollider upperCollider = upperTree.AddComponent<CapsuleCollider>();
            upperCollider.center = treeCollider.center + Vector3.up * (sliceHeight / 2);
            upperCollider.radius = treeCollider.radius;
            upperCollider.height = treeCollider.height - sliceHeight;

            treeInstances[i] = new TreeInstance();
        }
        terrain.terrainData.SetTreeInstances(treeInstances, false);

        TerrainCollider collider = terrain.GetComponent<TerrainCollider>();
        collider.enabled = false;
        collider.enabled = true;
    }

    public static TreeChopping GetInstance()
    {
        return instance;
    }
}
