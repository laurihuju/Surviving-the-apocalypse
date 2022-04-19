using UnityEngine;

public class DestroyTree : MonoBehaviour
{
    public int dropItemType;
    public int dropItemAmount;

    public float destroyDistanceFromCheckPoints;
    public LayerMask checkLayers;
    public Transform upperCheckPoint;
    public Transform lowerCheckPoint;

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
        GroundItemManager.GetInstance().AddGroundItem(dropItemType, dropItemAmount, transform.position);
        Destroy(gameObject);
    }
}
