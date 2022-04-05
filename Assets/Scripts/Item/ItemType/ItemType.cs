using UnityEngine;

public abstract class ItemType : MonoBehaviour
{
    [Tooltip("The item type id to register to the item system.")]
    [SerializeField] private int itemTypeID;

    [Tooltip("The maximum stack size of this type in inventory.")]
    [SerializeField] private int stackSize = 1;

    [Header("Prefab & sprite")]
    [Tooltip("The prefab to use when creating a new item of this type.")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Sprite sprite;

    /// <summary>
    /// Returns the type ID of this Item type.
    /// </summary>
    /// <returns></returns>
    public int GetTypeID()
    {
        return itemTypeID;
    }

    /// <summary>
    /// Returns the maximum stack size of this type in inventory.
    /// </summary>
    /// <returns></returns>
    public int GetStackSize()
    {
        return stackSize;
    }

    /// <summary>
    /// Returns the sprite of the item type.
    /// </summary>
    /// <returns></returns>
    public Sprite GetSprite()
    {
        return sprite;
    }

    /// <summary>
    /// Returns the ground item prefab of the item type.
    /// </summary>
    /// <returns></returns>
    public GameObject GetPrefab()
    {
        return itemPrefab;
    }

    /// <summary>
    /// Instantiates a new item of this type to the given location.
    /// </summary>
    /// <param name="position"></param>
    public virtual GameObject InstantiateItem(Vector3 position)
    {
        return Instantiate(itemPrefab, position, itemPrefab.transform.rotation);
    }

    /// <summary>
    /// Instantiates a new item of this type to the given location with the given rotation.
    /// </summary>
    /// <param name="position"></param>
    public virtual GameObject InstantiateItem(Vector3 position, Quaternion rotation)
    {
        return Instantiate(itemPrefab, position, rotation);
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
