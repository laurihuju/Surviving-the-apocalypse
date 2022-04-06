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
    [Tooltip("The layers that can contain obstacles that can prevent the object placing.")]
    [SerializeField] protected LayerMask obstacleLayers;

    [Header("Snapping")]
    [Tooltip("The IDs of the snappers to snap this object.")]
    [SerializeField] protected float[] snappers;
    [Tooltip("The layers that can contain snappers.")]
    [SerializeField] protected LayerMask snapperLayers;
    [Tooltip("The maximum range of snappers to snap.")]
    [SerializeField] protected Vector3 maxSnapRange;
    [Tooltip("If true, the item can be placed only on snappers.")]
    [SerializeField] protected bool requireSnapper;

    [Header("Other")]
    [Tooltip("The prefab to use when placing this object to the ground.")]
    [SerializeField] protected GameObject objectPrefab;

    protected Vector3 placementCheckOffset;
    protected Vector3 placementCheckSize;

    protected bool canPlace;
    protected ItemSnapper snapper;

    private void Start()
    {
        placementCheckOffset = new Vector3(objectWidth / 2, objectHeight / 2, objectWidth / -2);
        placementCheckSize = new Vector3(objectWidth / 2 - 0.01f, objectHeight / 2 - 0.01f, objectWidth / 2 - 0.01f);
    }

    public bool CanPlaceNoCheck()
    {
        return canPlace;
    }

    public virtual Vector3 GetPlaceLocation(Vector3 location, float yRotation)
    {
        snapper = null;
        if (snappers.Length > 0)
        {
            Collider[] possibleSnappers = Physics.OverlapBox(location, maxSnapRange, transform.rotation, snapperLayers.value);
            if (possibleSnappers.Length != 0)
            {
                for (int i = 0; i < possibleSnappers.Length; i++)
                {
                    ItemSnapper snapper = possibleSnappers[i].GetComponent<ItemSnapper>();
                    if (snapper == null)
                        continue;
                    if (!IsRightSnapper(snapper.GetID()))
                        continue;

                    location = snapper.GetPosition();
                    yRotation = snapper.GetRotation().eulerAngles.y;

                    this.snapper = snapper;
                    break;
                }
            } else if (requireSnapper)
            {
                canPlace = false;
                return location + Quaternion.Euler(0, yRotation, 0) * placementCheckOffset;
            }
        }

        Quaternion rotation = Quaternion.Euler(0, yRotation, 0);

        for (float y = 0; y <= maxPlaceHeight; y += 0.1f)
        {
            Vector3 checkLocation = location + Vector3.up * y + rotation * placementCheckOffset;

            if (Physics.OverlapBox(checkLocation, placementCheckSize, rotation, obstacleLayers.value).Length != 0)
                continue;

            if (!CanSnapNoCheck())
            {
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
            }
            canPlace = true;
            return checkLocation;
        }
        canPlace = false;
        return location + rotation * placementCheckOffset;
    }

    public virtual bool CanSnapNoCheck()
    {
        return snapper != null;
    }

    public virtual Quaternion GetSnapLocationNoCheck()
    {
        if (!CanSnapNoCheck())
            return Quaternion.Euler(0, 0, 0);
        return snapper.GetRotation();
    }

    public override GameObject InstantiateItem(Vector3 position)
    {
        return Instantiate(GetPrefab(), position, GetPrefab().transform.rotation);
    }

    public override GameObject InstantiateItem(Vector3 position, Quaternion rotation)
    {
        return Instantiate(objectPrefab, GetPlaceLocation(position, rotation.eulerAngles.y), rotation);
    }

    protected bool IsRightSnapper(int id)
    {
        for(int i = 0; i < snappers.Length; i++)
        {
            if (snappers[i] == id)
                return true;
        }
        return false;
    }
}
