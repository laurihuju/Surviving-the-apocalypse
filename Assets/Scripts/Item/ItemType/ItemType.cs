using UnityEngine;

public abstract class ItemType : MonoBehaviour
{
    [Tooltip("The item type id to register to the item system.")]
    [SerializeField] private int itemTypeID;

    [Tooltip("The prefab to use when creating a new item of this type.")]
    [SerializeField] private GameObject itemPrefab;

    /// <summary>
    /// Returns the type ID of this Item type.
    /// </summary>
    /// <returns></returns>
    public int GetTypeID()
    {
        return itemTypeID;
    }

    /// <summary>
    /// Instantiates a new item of this type to the given location.
    /// </summary>
    /// <param name="position"></param>
    public void InstantiateItem(Vector3 position)
    {
        Instantiate(itemPrefab, position, itemPrefab.transform.rotation);
    }

    /// <summary>
    /// Called when an item of this type is collected.
    /// </summary>
    public abstract void OnCollect();

    /// <summary>
    /// Called when an item of this type is dropped.
    /// </summary>
    public abstract void OnDrop();
}
