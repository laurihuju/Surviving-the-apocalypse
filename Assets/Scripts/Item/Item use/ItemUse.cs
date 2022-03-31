using UnityEngine;

public class ItemUse : MonoBehaviour
{
    private void Start()
    {
        InputManager.GetInstance().GetInputActions().Game.Use.performed += _ => UseItem();
    }

    private void UseItem()
    {
        ItemStack activeStack = Inventory.GetInstance().GetStack(HotBar.GetInstance().GetActiveSlot());
        if (activeStack == null)
            return;
        ItemType activeType = ItemTypeManager.GetInstance().GetItemType(activeStack.GetTypeID());
        if (activeType == null)
            return;
        UsableItem usableItem = activeType as UsableItem;
        if (usableItem == null)
            return;
        if (!usableItem.CanUse())
            return;
        usableItem.Use();
    }
}
