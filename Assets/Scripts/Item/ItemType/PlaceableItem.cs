using UnityEngine;

public abstract class PlaceableItem : ItemType
{
    public abstract bool CanPlaceNoCheck();

    public abstract Vector3 GetPlaceLocation(Vector3 location, float yRotation);

    public abstract bool CanSnapNoCheck();

    public abstract Quaternion GetSnapLocationNoCheck();

    public abstract GameObject PlaceItem(Vector3 position, Quaternion rotation);

    public abstract int GetPlaceItemID();

    public abstract void OnPlace(int slot, bool alreadyRemoved);
}
