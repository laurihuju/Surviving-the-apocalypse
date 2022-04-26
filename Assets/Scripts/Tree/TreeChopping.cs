using UnityEngine;
using System.Collections;
using EzySlice;

public class TreeChopping : MonoBehaviour
{
    private static TreeChopping instance;

    //Debug
    public Terrain terrain;
    public Transform testChopPoint;

    [Header("Chop checking")]
    [SerializeField] private float maxTreeCheckDistance;

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

        //StartCoroutine(Testi());
    }

    private IEnumerator Testi()
    {
        yield return new WaitForSeconds(5);
        ChopTree(terrain, testChopPoint.position);
    }

    public TreeInstance GetTreeNearPosition(Terrain terrain, Vector3 position)
    {
        if (position.y < terrain.SampleHeight(position) + 0.2f)
            return new TreeInstance();

        TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
        for (int i = 0; i < treeInstances.Length; i++)
        {
            //Check if the current tree is near to the chop position.
            Vector3 treePosition = Vector3.Scale(treeInstances[i].position, terrain.terrainData.size) + terrain.transform.position;
            if (Mathf.Abs(treePosition.x - position.x) > maxTreeCheckDistance)
                continue;
            if (Mathf.Abs(treePosition.z - position.z) > maxTreeCheckDistance)
                continue;
            return treeInstances[i];
        }
        return new TreeInstance();
    }

    public void ChopTree(Terrain terrain, Vector3 exactTreePosition)
    {
        TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
        for(int i = 0; i < treeInstances.Length; i++)
        {
            //Check if the current tree is in the chop position.
            if (treeInstances[i].position != exactTreePosition)
                continue;
            Vector3 treePosition = Vector3.Scale(treeInstances[i].position, terrain.terrainData.size) + terrain.transform.position;

            //Find the prefab and collider of the tree to chop and slice the tree.
            GameObject treePrefab = terrain.terrainData.treePrototypes[treeInstances[i].prototypeIndex].prefab;
            treePrefab.transform.position = treePosition;
            CapsuleCollider treeCollider = treePrefab.GetComponent<CapsuleCollider>();
            if (treeCollider == null)
                continue;
            SlicedHull slicedTree = treePrefab.Slice(treePosition + Vector3.up * sliceHeight, Vector3.up);

            //Lower tree spawning and settings
            GameObject lowerTree = slicedTree.CreateLowerHull(treePrefab, treeMaterial);
            float treeBottomHeight = lowerTree.transform.position.y + treeCollider.center.y - treeCollider.height / 2;
            float realSliceHeight = treePosition.y + sliceHeight - treeBottomHeight;

            CapsuleCollider lowerCollider = lowerTree.AddComponent<CapsuleCollider>();
            lowerCollider.center = new Vector3(treeCollider.center.x, (treeBottomHeight + realSliceHeight / 2) - lowerTree.transform.position.y, treeCollider.center.z);
            if (treeCollider.radius * 2 <= realSliceHeight)
                lowerCollider.radius = treeCollider.radius;
            else
                lowerCollider.radius = realSliceHeight / 2;
            lowerCollider.height = realSliceHeight;

            //Upper tree spawning
            GameObject upperTree = slicedTree.CreateUpperHull(treePrefab, treeMaterial);

            //Upper tree settings
            upperTree.layer = fallingTreeLayer;

            Rigidbody upperRb = upperTree.AddComponent<Rigidbody>();
            upperRb.constraints = RigidbodyConstraints.FreezeRotationY;
            upperRb.mass = fallingTreeMass;
            upperRb.AddForce(Vector3.Normalize(treePosition - PlayerController.GetInstance().transform.position) * treeFallForce, ForceMode.Impulse);

            CapsuleCollider upperCollider = upperTree.AddComponent<CapsuleCollider>();
            upperCollider.center = treeCollider.center + Vector3.up * (realSliceHeight / 2);
            upperCollider.radius = treeCollider.radius;
            upperCollider.height = treeCollider.height - realSliceHeight;

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
            Transform destroyTreeBottomPoint = new GameObject().transform;
            destroyTreeBottomPoint.parent = upperTree.transform;
            destroyTreeBottomPoint.localPosition = upperCollider.center + Vector3.down * (upperCollider.height / 2);
            destroyTree.treeBottom = destroyTreeBottomPoint;
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
