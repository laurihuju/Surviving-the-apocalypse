using UnityEngine;

public abstract class PlaceableItem : ItemType
{
    [Header("Placement")]
    [Tooltip("The width of the placeable object.")]
    [SerializeField] protected float objectWidth;
    [Tooltip("The height of the placeable object.")]
    [SerializeField] protected float objectHeight;
    [Tooltip("The maximum height from ground to allow object placement.")]
    [SerializeField] [Range(0, 2)] protected float maxPlaceHeight;

    private Vector3 placementCheckOffset;
    private Vector3 placementCheckSize;

    protected bool canPlace;

    private void Start()
    {
        placementCheckOffset = new Vector3(objectWidth / 2, objectHeight / 2, objectWidth / -2);
        placementCheckSize = new Vector3(objectWidth / 2, objectHeight / 2, objectWidth / 2);
    }

    public virtual bool CanPlace(Vector3 location, float yRotation)
    {
        Quaternion rotation = Quaternion.Euler(0, yRotation, 0);

        for (float y = 0; y <= maxPlaceHeight; y += 0.1f)
        {
            Vector3 checkLocation = location + Vector3.up * y + rotation * placementCheckOffset;

            if (Physics.OverlapBox(checkLocation, placementCheckSize, rotation).Length != 0)
                continue;

            if (!Physics.Raycast(location + Vector3.up * y + rotation * new Vector3(objectWidth, 0, 0), Vector3.down, maxPlaceHeight))
            {
                canPlace = false;
                return false;
            }
            if (!Physics.Raycast(location + Vector3.up * y + rotation * new Vector3(0, 0, -objectWidth), Vector3.down, maxPlaceHeight))
            {
                canPlace = false;
                return false;
            }
            if (!Physics.Raycast(location + Vector3.up * y + rotation * new Vector3(objectWidth, 0, -objectWidth), Vector3.down, maxPlaceHeight))
            {
                canPlace = false;
                return false;
            }
            canPlace = true;
            return true;
        }
        canPlace = false;
        return false;
    }

    public bool CanPlaceNoCheck()
    {
        return canPlace;
    }

    public virtual Vector3 GetPlaceLocation(Vector3 location, float yRotation)
    {
        Quaternion rotation = Quaternion.Euler(0, yRotation, 0);

        for (float y = 0; y <= maxPlaceHeight; y += 0.1f)
        {
            Vector3 checkLocation = location + Vector3.up * y + rotation * placementCheckOffset;

            if (Physics.OverlapBox(checkLocation, placementCheckSize, rotation).Length != 0)
                continue;

            if (!Physics.Raycast(location + Vector3.up * y + rotation * new Vector3(objectWidth, 0, 0), Vector3.down, maxPlaceHeight))
            {
                canPlace = false;
                return location + rotation * placementCheckOffset;
            }
            if (!Physics.Raycast(location + Vector3.up * y + rotation * new Vector3(0, 0, -objectWidth), Vector3.down, maxPlaceHeight))
            {
                canPlace = false;
                return location + rotation * placementCheckOffset;
            }
            if (!Physics.Raycast(location + Vector3.up * y + rotation * new Vector3(objectWidth, 0, -objectWidth), Vector3.down, maxPlaceHeight))
            {
                canPlace = false;
                return location + rotation * placementCheckOffset;
            }
            canPlace = true;
            return checkLocation;
        }
        canPlace = false;
        return location + rotation * placementCheckOffset;
    }

    public override GameObject InstantiateItem(Vector3 position)
    {
        return Instantiate(GetPrefab(), position, GetPrefab().transform.rotation);
    }

    public override GameObject InstantiateItem(Vector3 position, Quaternion rotation)
    {
        return Instantiate(GetPrefab(), GetPlaceLocation(position, rotation.eulerAngles.y), rotation);
    }
}
