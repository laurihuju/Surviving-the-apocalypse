using UnityEngine;

public class ItemPlace : MonoBehaviour
{
    [Tooltip("The maximum distance from player to allow item placing.")]
    [SerializeField] private float maxPlaceDistance;

    private Transform previewTransform;

    private void Start()
    {
        InputManager.GetInstance().GetInputActions().Game.Use.performed += _ => PlaceItem();

        Inventory.GetInstance().AddItem(5738, 1);
    }

    private void PlaceItem()
    {
        PlaceableItem placeableItem = GetCurrentItem();
        if (placeableItem == null)
            return;
        Vector3 placeLocation = GetPlaceLocation();
        if (placeLocation == Vector3.zero)
            return;
        if (!placeableItem.CanPlace(placeLocation, PlayerController.GetInstance().transform.rotation.eulerAngles.y))
            return;
        GroundItemManager.GetInstance().AddGroundItem(placeableItem.GetTypeID(), 1, placeLocation, PlayerController.GetInstance().transform.rotation);
    }

    private PlaceableItem GetCurrentItem()
    {
        ItemStack activeStack = Inventory.GetInstance().GetStack(HotBar.GetInstance().GetActiveSlot());
        if (activeStack == null)
            return null;
        ItemType activeType = ItemTypeManager.GetInstance().GetItemType(activeStack.GetTypeID());
        if (activeType == null)
            return null;
        return activeType as PlaceableItem;
    }

    private Vector3 GetPlaceLocation()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo, maxPlaceDistance))
            return hitInfo.point;
        return Vector3.zero;
    }
}
