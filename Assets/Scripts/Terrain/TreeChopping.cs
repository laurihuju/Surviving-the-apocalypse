using UnityEngine;
using System.Collections;
using EzySlice;

public class TreeChopping : MonoBehaviour
{
    private static TreeChopping instance;

    //Debug
    public Terrain terrain;
    public Transform testChopPoint;

    [Header("Slicing")]
    [SerializeField] private float sliceHeight;
    [SerializeField] private Material treeMaterial;

    [Header("Tree Falling")]
    [SerializeField] private float treeFallForce;
    [SerializeField] private float fallingTreeMass;
    [SerializeField] private int fallingTreeLayer;
    [SerializeField] private LayerMask fallingCheckLayers;

    [Header("Tree drops")]
    [SerializeField] private int dropItemType;
    [SerializeField] private int itemDropAmount;

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
            //Check if the current tree is near to the chop position.
            Vector3 treePosition = Vector3.Scale(treeInstances[i].position, terrain.terrainData.size) + terrain.transform.position;
            if (Mathf.Abs(treePosition.x - position.x) > 1)
                continue;
            if (Mathf.Abs(treePosition.z - position.z) > 1)
                continue;

            //Find the prefab and collider of the tree to chop and slice the tree.
            GameObject treePrefab = terrain.terrainData.treePrototypes[treeInstances[i].prototypeIndex].prefab;
            CapsuleCollider treeCollider = treePrefab.GetComponent<CapsuleCollider>();
            if (treeCollider == null)
                continue;
            SlicedHull slicedTree = treePrefab.Slice(treePosition + Vector3.up * sliceHeight, Vector3.up);

            //Lower tree spawning
            slicedTree.CreateLowerHull(treePrefab, treeMaterial).AddComponent<CapsuleCollider>();
            
            //Upper tree spawning
            GameObject upperTree = slicedTree.CreateUpperHull(treePrefab, treeMaterial);

            //Upper tree settings
            upperTree.layer = fallingTreeLayer;

            Rigidbody upperRb = upperTree.AddComponent<Rigidbody>();
            upperRb.constraints = RigidbodyConstraints.FreezeRotationY;
            upperRb.mass = fallingTreeMass;
            upperRb.AddForce(Vector3.Normalize(treePosition - PlayerController.GetInstance().transform.position) * treeFallForce, ForceMode.Impulse);
            
            CapsuleCollider upperCollider = upperTree.AddComponent<CapsuleCollider>();
            upperCollider.center = treeCollider.center + Vector3.up * (sliceHeight / 2);
            upperCollider.radius = treeCollider.radius;
            upperCollider.height = treeCollider.height - sliceHeight;

            DestroyTree destroyTree = upperTree.AddComponent<DestroyTree>();
            destroyTree.dropItemType = dropItemType;
            destroyTree.dropItemAmount = itemDropAmount;
            Transform destroyTeeLowerCheckPoint = new GameObject().transform;
            destroyTeeLowerCheckPoint.parent = upperTree.transform;
            destroyTeeLowerCheckPoint.localPosition = upperCollider.center;
            destroyTree.lowerCheckPoint = destroyTeeLowerCheckPoint;
            Transform destroyTreeUpperCheckPoint = new GameObject().transform;
            destroyTreeUpperCheckPoint.parent = upperTree.transform;
            destroyTreeUpperCheckPoint.localPosition = upperCollider.center + Vector3.up * (upperCollider.height / 2);
            destroyTree.upperCheckPoint = destroyTreeUpperCheckPoint;
            destroyTree.destroyDistanceFromCheckPoints = upperCollider.radius + 0.01f;
            destroyTree.checkLayers = fallingCheckLayers;

            //Remove current tree
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
