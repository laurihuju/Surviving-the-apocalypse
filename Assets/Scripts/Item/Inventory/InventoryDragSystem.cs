using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryDragSystem : MonoBehaviour
{
    private static InventoryDragSystem instance;

    private InventoryItemDrag currentDragger;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    /// <summary>
    /// Drags the current dragger if it is not null.
    /// </summary>
    private void Update()
    {
        if (currentDragger == null)
            return;

        currentDragger.transform.position = Mouse.current.position.ReadValue();
    }

    /// <summary>
    /// Set the InventoryItemDrag that should be dragged currently. Set this null to stop dragging.
    /// </summary>
    /// <param name="dragger"></param>
    public void SetDragging(InventoryItemDrag dragger)
    {
        currentDragger = dragger;
    }

    /// <summary>
    /// Returns the current InventoryItemDrag being dragged.
    /// </summary>
    /// <returns></returns>
    public InventoryItemDrag GetCurrentDragger()
    {
        return currentDragger;
    }

    /// <summary>
    /// Returns the singleton instance of InventoryDragSystem.
    /// </summary>
    /// <returns></returns>
    public static InventoryDragSystem GetInstance()
    {
        return instance;
    }
}
