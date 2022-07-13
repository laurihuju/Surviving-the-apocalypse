using UnityEngine;

public class DefaultPlaceableItem : PlaceableItem
{
    [Header("Placement")]
    //[Tooltip("The width of the placeable object on the x axis.")]
    //[SerializeField] protected float objectWidthX;
    //[Tooltip("The width of the placeable object on the z axis.")]
    //[SerializeField] protected float objectWidthZ;
    //[Tooltip("The height of the placeable object.")]
    //[SerializeField] protected float objectHeight;
    [SerializeField] protected Vector3 offsetFromPlacementCornerToCenter;
    [SerializeField] protected Vector3 offsetFromPlacementCornerToPlacementPoint;
    [SerializeField] protected Vector3 placementCheckSize;
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
    [Tooltip("Should this item be removed from the inventory on use.")]
    [SerializeField] protected bool removeFromInventory;

    protected bool canPlace;
    protected ItemSnapper snapper;

    private void Start()
    {
        /*placementCheckOffset = new Vector3(objectWidthX / 2, objectHeight / 2, objectWidthZ / -2);
        placementCheckSize = new Vector3(objectWidthX / 2 - 0.01f, objectHeight / 2 - 0.01f, objectWidthZ / 2 - 0.01f);

        offsetFromLeftCornerToPlacementPoint = offsetFromLeftCornerToCenter;
        for (int i = 0; i < objectPrefab.transform.childCount; i++)
        {
            Transform child = objectPrefab.transform.GetChild(i);
            if (!child.CompareTag("ObjectCenter"))
                continue;
            offsetFromLeftCornerToPlacementPoint = offsetFromLeftCornerToCenter + (transform.position - child.transform.position);
            break;
        }*/
    }

    public override bool CanPlaceNoCheck()
    {
        return canPlace;
    }

    public override Vector3 GetPlaceLocation(Vector3 location, float yRotation)
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
            }
            else if (requireSnapper)
            {
                canPlace = false;
                return location + Quaternion.Euler(0, yRotation, 0) * offsetFromPlacementCornerToPlacementPoint;
            }
        }

        Quaternion rotation = Quaternion.Euler(0, yRotation, 0);

        for (float y = 0; y <= maxPlaceHeight; y += 0.1f)
        {
            Vector3 checkLocation = location + Vector3.up * y + rotation * offsetFromPlacementCornerToCenter;

            Debug.DrawLine(checkLocation, location, Color.black);

            if (Physics.OverlapBox(checkLocation, placementCheckSize, rotation, obstacleLayers.value).Length != 0)
                continue;

            if (!CanSnapNoCheck())
            {
                if (!Physics.Raycast(location + Vector3.up * y + rotation * new Vector3(offsetFromPlacementCornerToCenter.x * 2, 0, 0), Vector3.down, maxPlaceHeight))
                {
                    canPlace = false;
                    return location + rotation * offsetFromPlacementCornerToPlacementPoint;
                }
                if (!Physics.Raycast(location + Vector3.up * y + rotation * new Vector3(0, 0, offsetFromPlacementCornerToCenter.z * 2), Vector3.down, maxPlaceHeight))
                {
                    canPlace = false;
                    return location + rotation * offsetFromPlacementCornerToPlacementPoint;
                }
                if (!Physics.Raycast(location + Vector3.up * y + rotation * new Vector3(offsetFromPlacementCornerToCenter.x * 2, 0, offsetFromPlacementCornerToCenter.z * 2), Vector3.down, maxPlaceHeight))
                {
                    canPlace = false;
                    return location + rotation * offsetFromPlacementCornerToPlacementPoint;
                }
            }
            canPlace = true;
            return location + Vector3.up * y + rotation * offsetFromPlacementCornerToPlacementPoint;
        }
        canPlace = false;
        return location + rotation * offsetFromPlacementCornerToPlacementPoint;
    }

    public override bool CanSnapNoCheck()
    {
        return snapper != null;
    }

    public override Quaternion GetSnapLocationNoCheck()
    {
        if (!CanSnapNoCheck())
            return Quaternion.Euler(0, 0, 0);
        return snapper.GetRotation();
    }

    public override bool ShowBase()
    {
        if (!CanSnapNoCheck())
            return true;
        return snapper.ShowBase();
    }

    public override GameObject PlaceItem(Vector3 position, Quaternion rotation)
    {
        return Instantiate(objectPrefab, GetPlaceLocation(position, rotation.eulerAngles.y), rotation);
    }

    protected bool IsRightSnapper(int id)
    {
        for (int i = 0; i < snappers.Length; i++)
        {
            if (snappers[i] == id)
                return true;
        }
        return false;
    }

    public override void OnCollect()
    {

    }

    public override void OnDrop()
    {

    }

    public override void OnSelect()
    {

    }

    public override void OnDeselect()
    {

    }

    public override int GetPlaceItemID()
    {
        return GetTypeID();
    }

    public override void OnPlace(int slot, bool alreadyRemoved)
    {
        if (!removeFromInventory || alreadyRemoved)
            return;
        Inventory.GetInstance().RemoveItemFromSlot(slot, 1);
    }
}
