using UnityEngine;

public class PlacedItem : MonoBehaviour
{
    [SerializeField] private int typeID;

    [SerializeField] private GameObject itemBase;

    public GroundItemData GenerateData()
    {
        if(itemBase != null)
            return new GroundItemData(typeID, transform.position, transform.rotation, itemBase.activeSelf);
        return new GroundItemData(typeID, transform.position, transform.rotation, false);
    }

    public GameObject GetItemBase()
    {
        return itemBase;
    }

    /// <summary>
    /// Returns the type ID of this ground item.
    /// </summary>
    /// <returns></returns>
    public int GetTypeID()
    {
        return typeID;
    }
}
