using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private int slotIndex;

    private Vector3 originalPosition;

    /// <summary>
    /// Sets the original slot position.
    /// </summary>
    private void Start()
    {
        originalPosition = transform.position;
    }

    /// <summary>
    /// Starts item dragging.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.parent.SetAsLastSibling();

        InventoryDragSystem.GetInstance().SetDragging(this);
    }

    /// <summary>
    /// Stops item dragging, sets the dragged slot back to it's original position and swaps the slot content if the item dragged over another slot.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        InventoryDragSystem.GetInstance().GetCurrentDragger().MoveToOriginalPosition();
        InventoryDragSystem.GetInstance().SetDragging(null);

        InventoryUISlot slotInPosition = Inventory.GetInstance().GetSlotInClickPosition(eventData);
        if(slotInPosition != null)
        {
            Inventory.GetInstance().SwapSlots(slotIndex, slotInPosition.GetSlotIndex());
        }
    }

    /// <summary>
    /// Returns the slot index of this InventoryItemDrag.
    /// </summary>
    /// <returns></returns>
    public int GetSlotIndex()
    {
        return slotIndex;
    }

    /// <summary>
    /// Moves InventoryItemDrag to it's original position.
    /// </summary>
    public void MoveToOriginalPosition()
    {
        transform.position = originalPosition;
    }
}
