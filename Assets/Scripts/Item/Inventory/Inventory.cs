using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    private static Inventory instance;

    [Tooltip("The amount of slots in the inventory.")]
    [SerializeField] private int inventorySize;

    [Header("UI")]
    [SerializeField] private InventoryUISlot[] slots;

    private ItemStack[] items;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        items = new ItemStack[inventorySize];
    }

    /// <summary>
    /// Tries to add given amount of given item type to the inventory. The method returns the amount of items that were added to the inventory. If there is no space enough, the return amount will be smaller than the given add amount.
    /// </summary>
    /// <param name="typeID"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public int AddItem(int typeID, int amount)
    {
        ItemType type = ItemTypeManager.GetInstance().GetItemType(typeID);
        if (type == null)
            return 0;

        int addedAmount = 0;
        for(int i = 0; i < inventorySize; i++)
        {
            int slot = FindSlotForItem(type);
            if (slot == -1)
                return addedAmount;
            if (items[slot] == null)
            {
                items[slot] = new ItemStack(typeID);
                slots[slot].SetSlotImage(type.GetSprite());
            }

            int maxAmountToAdd = type.GetStackSize() - items[slot].GetAmount();
            if (maxAmountToAdd >= amount)
            {
                items[slot].SetAmount(items[slot].GetAmount() + amount);
                slots[slot].SetSlotAmountText(items[slot].GetAmount());
                return addedAmount + amount;
            }
            items[slot].SetAmount(items[slot].GetAmount() + maxAmountToAdd);
            slots[slot].SetSlotAmountText(items[slot].GetAmount());
            amount -= maxAmountToAdd;
            addedAmount += maxAmountToAdd;
        }
        return addedAmount;
    }

    /// <summary>
    /// Tries to remove given amount of given item type from the inventory. The method returns the amount of items that were removed from the inventory. If there are no items enough, the return amount will be smaller than the given remove amount.
    /// </summary>
    /// <param name="typeID"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public int RemoveItem(int typeID, int amount)
    {
        ItemType type = ItemTypeManager.GetInstance().GetItemType(typeID);
        if (type == null)
            return 0;

        int removedAmount = 0;
        for (int i = 0; i < inventorySize; i++)
        {
            int slot = FindSlotWithItem(type);
            if (slot == -1)
                return removedAmount;

            int maxAmountToRemove = items[slot].GetAmount();
            if(maxAmountToRemove >= amount)
            {
                items[slot].SetAmount(items[slot].GetAmount() - amount);
                slots[slot].SetSlotAmountText(items[slot].GetAmount());
                if (items[slot].GetAmount() <= 0)
                {
                    items[slot] = null;
                    slots[slot].SetSlotImage(null);
                }
                return removedAmount + amount;
            }
            items[slot].SetAmount(items[slot].GetAmount() - maxAmountToRemove);
            slots[slot].SetSlotAmountText(items[slot].GetAmount());
            if (items[slot].GetAmount() <= 0)
            {
                items[slot] = null;
                slots[slot].SetSlotImage(null);
            }
            amount -= maxAmountToRemove;
            removedAmount += maxAmountToRemove;
        }
        return removedAmount;
    }

    /// <summary>
    /// Tries to remove given amount of items from the given slot. The method returns the amount of items that were removed from the slot. If there are no items enough, the return amount will be smaller than the given remove amount.
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public int RemoveItemFromSlot(int slot, int amount)
    {
        if (items[slot] == null)
            return 0;

        int maxAmountToRemove = items[slot].GetAmount();
        if(maxAmountToRemove >= amount)
        {
            items[slot].SetAmount(items[slot].GetAmount() - amount);
            slots[slot].SetSlotAmountText(items[slot].GetAmount());
            if (items[slot].GetAmount() <= 0)
            {
                items[slot] = null;
                slots[slot].SetSlotImage(null);
            }
            return amount;
        }
        items[slot].SetAmount(items[slot].GetAmount() - maxAmountToRemove);
        slots[slot].SetSlotAmountText(items[slot].GetAmount());
        if (items[slot].GetAmount() <= 0)
        {
            items[slot] = null;
            slots[slot].SetSlotImage(null);
        }
        return maxAmountToRemove;
    }

    /// <summary>
    /// Swaps the content in two slots.
    /// </summary>
    /// <param name="slot1"></param>
    /// <param name="slot2"></param>
    public void SwapSlots(int slot1, int slot2)
    {
        if (slot1 >= inventorySize || slot2 >= inventorySize || slot1 < 0 || slot2 < 0 || slot1 == slot2)
            return;

        ItemStack slot1Item = items[slot1];

        items[slot1] = items[slot2];
        items[slot2] = slot1Item;

        if(items[slot1] != null)
        {
            ItemType slot1Type = ItemTypeManager.GetInstance().GetItemType(items[slot1].GetTypeID());
            slots[slot1].SetSlotImage(slot1Type.GetSprite());
            slots[slot1].SetSlotAmountText(items[slot1].GetAmount());
        } else
        {
            slots[slot1].SetSlotImage(null);
            slots[slot1].SetSlotAmountText(0);
        }

        if (items[slot2] != null)
        {
            ItemType slot2Type = ItemTypeManager.GetInstance().GetItemType(items[slot2].GetTypeID());
            slots[slot2].SetSlotImage(slot2Type.GetSprite());
            slots[slot2].SetSlotAmountText(items[slot2].GetAmount());
        } else
        {
            slots[slot2].SetSlotImage(null);
            slots[slot2].SetSlotAmountText(0);
        }
    }

    /// <summary>
    /// Returns the inventory slot in the given eventData click position. Returns null if slot didn't found.
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    public InventoryUISlot GetSlotInClickPosition(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (!result.gameObject.CompareTag("InventorySlot"))
                continue;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].gameObject == result.gameObject)
                {
                    return slots[i];
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Returns the amount of items of the given type that can be added to the inventory.
    /// </summary>
    /// <param name="typeID"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public int GetSpaceForItem(int typeID, int amount)
    {
        ItemType type = ItemTypeManager.GetInstance().GetItemType(typeID);
        if (type == null)
            return 0;

        int space = 0;
        for(int i = 0; i < items.Length; i++)
        {
            if(items[i] == null)
            {
                space += type.GetStackSize();
                continue;
            }
            if (items[i].GetTypeID() != typeID)
                continue;
            space += type.GetStackSize() - items[i].GetAmount();
        }
        return space;
    }

    /// <summary>
    /// Tries to find a slot that has the given item and has at least 1 space left. If slot didn't found, the method tries to find an empty slot. If any slot didn't found, the method will return -1.
    /// </summary>
    /// <param name="typeID"></param>
    /// <returns></returns>
    private int FindSlotForItem(ItemType type)
    {
        for(int i = 0; i < inventorySize; i++)
        {
            if (items[i] == null)
                continue;
            if (items[i].GetTypeID() != type.GetTypeID())
                continue;
            if (items[i].GetAmount() >= type.GetStackSize())
                continue;
            return i;
        }

        for (int i = 0; i < inventorySize; i++)
        {
            if (items[i] == null)
                return i;
        }

        return -1;
    }

    private int FindSlotWithItem(ItemType type)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            if (items[i] == null)
                continue;
            if (items[i].GetTypeID() == type.GetTypeID())
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the singleton instance of Inventory class.
    /// </summary>
    /// <returns></returns>
    public static Inventory GetInstance()
    {
        return instance;
    }
}
