using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPlace : MonoBehaviour
{
    [Tooltip("The maximum distance from player to allow item placing.")]
    [SerializeField] private float maxPlaceDistance;

    private void Start()
    {
        InputManager.GetInstance().GetInputActions().Game.Use.performed += _ => PlaceItem();

        Inventory.GetInstance().AddItem(5738, 1);
    }

    private void PlaceItem()
    {
        ItemStack activeStack = Inventory.GetInstance().GetStack(HotBar.GetInstance().GetActiveSlot());
        if (activeStack == null)
            return;
        ItemType activeType = ItemTypeManager.GetInstance().GetItemType(activeStack.GetTypeID());
        if (activeType == null)
            return;
        PlaceableItem placeableItem = activeType as PlaceableItem;
        if (placeableItem == null)
            return;
        Vector3 placeLocation = GetPlaceLocation();
        if (placeLocation == Vector3.zero)
            return;
        if (!placeableItem.CanPlace(placeLocation))
            return;
        placeableItem.InstantiateItem(placeLocation);
    }

    private Vector3 GetPlaceLocation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo, maxPlaceDistance))
            return hitInfo.point;
        return Vector3.zero;
    }
}
