using UnityEngine;

public class DestroyTree : MonoBehaviour
{
    public int dropItemType;
    public int dropItemAmount;

    public float destroyDistanceFromCheckPoints;
    public LayerMask checkLayers;
    public Transform upperCheckPoint;
    public Transform lowerCheckPoint;

    public Transform treeBottom;

    private void Update()
    {
        Collider[] colliders = Physics.OverlapCapsule(upperCheckPoint.position, lowerCheckPoint.position, destroyDistanceFromCheckPoints, checkLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Terrain"))
            {
                RemoveTree();
                return;
            }
        }
    }

    private void RemoveTree()
    {
        GroundItemManager.GetInstance().AddSeparateGroundItemsItems(dropItemType, GetDropLocations());
        Destroy(gameObject);
    }

    private Vector3[] GetDropLocations()
    {
        float dropDistance = (lowerCheckPoint.localPosition.y - treeBottom.localPosition.y) / dropItemAmount;

        Vector3[] dropLocations = new Vector3[dropItemAmount];
        for(int i = 0; i < dropItemAmount; i++)
        {
            dropLocations[i] = treeBottom.transform.position + transform.up * dropDistance * i;
        }
        return dropLocations;
    }
}
