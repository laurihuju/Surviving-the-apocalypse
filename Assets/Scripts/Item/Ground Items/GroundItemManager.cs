using System.Collections.Generic;
using UnityEngine;

public class GroundItemManager : MonoBehaviour
{
    private static GroundItemManager instance;

    private List<GroundItem> groundItems;
    private List<PlacedItem> placedItems;

    [Header("Torch")]
    [SerializeField] private int torchItemType;
    [SerializeField] private float torchLightingDistance;
    [SerializeField] private float torchLightLevelInCenter;
    [SerializeField] private float torchLightLevelInBorder;

    public float TorchLightingDistance { get => torchLightingDistance;}

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        groundItems = new List<GroundItem>();
        placedItems = new List<PlacedItem>();
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

        groundItems.Add(instantiatedItem);
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

        groundItems.Add(instantiatedItem);
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

            groundItems.Add(instantiatedItem);
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

            groundItems.Add(instantiatedItem);
        }
    }

    public void PlaceItem(PlaceableItem type, Vector3 position, Quaternion rotation)
    {
        PlacedItem instantiatedItem = type.PlaceItem(position, rotation).GetComponent<PlacedItem>();
        if (instantiatedItem == null)
            return;
        GameObject itemBase = instantiatedItem.GetItemBase();
        if (itemBase != null)
            itemBase.SetActive(type.ShowBase());

        placedItems.Add(instantiatedItem);
    }

    public float GetTorchLightLevelInPosition(Vector3 position)
    {
        float distanceToNearestTorch = -1;
        for (int i = 0; i < placedItems.Count; i++)
        {
            if (placedItems[i].GetTypeID() != torchItemType)
                continue;
            float currentDistance = Vector3.Distance(position, placedItems[i].transform.position);
            if (currentDistance < distanceToNearestTorch || distanceToNearestTorch == -1)
                distanceToNearestTorch = currentDistance;
        }
        if (distanceToNearestTorch == -1 || distanceToNearestTorch > TorchLightingDistance)
            return 0;
        return Mathf.Lerp(torchLightLevelInCenter, torchLightLevelInBorder, distanceToNearestTorch / TorchLightingDistance);
    }

    /// <summary>
    /// Unregisters a ground item so it won't be saved when saving the game. Call this when item is no longer in the ground.
    /// </summary>
    /// <param name="item"></param>
    public void UnregisterGroundItem(GroundItem item)
    {
        groundItems.Remove(item);
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
