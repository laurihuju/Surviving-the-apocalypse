using UnityEngine;

public abstract class PlaceableItem : ItemType
{
    [Header("Placement")]
    [Tooltip("The width of the placeable object.")]
    [SerializeField] protected float objectWidth;
    [Tooltip("The height of the placeable object.")]
    [SerializeField] protected float objectHeight;

    private Vector3 placementCheckOffset;

    private void Start()
    {
        placementCheckOffset = new Vector3(objectWidth / 2, objectHeight / 2, objectWidth / 2);
    }

    public virtual bool CanPlace(Vector3 location, Quaternion rotation)
    {
        return Physics.OverlapBox(location + placementCheckOffset, new Vector3(objectWidth, objectHeight, objectWidth), rotation).Length == 0;
    }

    public override GameObject InstantiateItem(Vector3 position)
    {
        return Instantiate(GetPrefab(), position, GetPrefab().transform.rotation);
    }

    public override GameObject InstantiateItem(Vector3 position, Quaternion rotation)
    {
        return Instantiate(GetPrefab(), position, rotation);
    }
}
