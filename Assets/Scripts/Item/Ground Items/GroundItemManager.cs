using System.Collections.Generic;
using UnityEngine;

public class GroundItemManager : MonoBehaviour
{
    private static GroundItemManager instance;

    private List<GroundItem> items;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        items = new List<GroundItem>();
    }

    /// <summary>
    /// Instantiates items to one stack in the scene.
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="amount"></param>
    /// <param name="position"></param>
    public void AddGroundItem(int itemType, int amount, Vector3 position)
    {
        ItemType type = ItemTypeManager.GetInstance().GetItemType(itemType);
        if (type == null)
            return;
        GroundItem instantiatedItem = type.InstantiateItem(position).GetComponent<GroundItem>();
        if (instantiatedItem == null)
            return;
        instantiatedItem.SetAmount(amount);

        items.Add(instantiatedItem);
    }

    /// <summary>
    /// Instantiates items to one stack in the scene with specified rotation.
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="amount"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void AddGroundItem(int itemType, int amount, Vector3 position, Quaternion rotation)
    {
        ItemType type = ItemTypeManager.GetInstance().GetItemType(itemType);
        if (type == null)
            return;
        GroundItem instantiatedItem = type.InstantiateItem(position, rotation).GetComponent<GroundItem>();
        if (instantiatedItem == null)
            return;
        instantiatedItem.SetAmount(amount);

        items.Add(instantiatedItem);
    }

    /// <summary>
    /// Instantiates items to separate positions in the scene.
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="positions"></param>
    public void AddSeparateGroundItemsItems(int itemType, Vector3[] positions)
    {
        ItemType type = ItemTypeManager.GetInstance().GetItemType(itemType);
        if (type == null)
            return;

        for(int i = 0; i < positions.Length; i++)
        {
            GroundItem instantiatedItem = type.InstantiateItem(positions[i]).GetComponent<GroundItem>();
            if (instantiatedItem == null)
                return;

            items.Add(instantiatedItem);
        }
    }

    /// <summary>
    /// Instantiates items to separate positions and rotations in the scene.
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="positions"></param>
    /// <param name="rotations"></param>
    public void AddSeparateGroundItemsItems(int itemType, Vector3[] positions, Quaternion[] rotations)
    {
        ItemType type = ItemTypeManager.GetInstance().GetItemType(itemType);
        if (type == null)
            return;

        for (int i = 0; i < positions.Length; i++)
        {
            if (i >= rotations.Length)
                break;
            GroundItem instantiatedItem = type.InstantiateItem(positions[i], rotations[i]).GetComponent<GroundItem>();
            if (instantiatedItem == null)
                return;

            items.Add(instantiatedItem);
        }
    }

    /// <summary>
    /// Unregisters a ground item so it won't be saved when saving the game. Call this when item is no longer in the ground.
    /// </summary>
    /// <param name="item"></param>
    public void UnregisterGroundItem(GroundItem item)
    {
        items.Remove(item);
    }

    /// <summary>
    /// Returns the singleton instance of the GroundItemManager class.
    /// </summary>
    /// <returns></returns>
    public static GroundItemManager GetInstance()
    {
        return instance;
    }
}
