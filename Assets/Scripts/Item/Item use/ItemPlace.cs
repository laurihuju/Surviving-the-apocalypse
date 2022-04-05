using UnityEngine;

public class ItemPlace : MonoBehaviour
{
    [Tooltip("The maximum distance from player to allow item placing.")]
    [SerializeField] private float maxPlaceDistance;

    [Tooltip("The material to apply to the placement previews (blueprints).")]
    [SerializeField] private Material blueprintMaterial;
    [Tooltip("The color of the placement preview (blueprint) when the item can be placed.")]
    [SerializeField] private Color blueprintCanPlaceColor;
    [Tooltip("The color of the placement preview (blueprint) when the item cannot be placed.")]
    [SerializeField] private Color blueprintCannotPlaceColor;

    private Transform previewTransform;

    private void Start()
    {
        InputManager.GetInstance().GetInputActions().Game.Use.performed += _ => PlaceItem();

        Inventory.GetInstance().AddItem(5738, 1);
    }

    private void Update()
    {
        PlaceableItem currentItem = GetCurrentItem();
        if (currentItem == null)
        {
            if (previewTransform == null)
                return;
            Destroy(previewTransform.gameObject);
            previewTransform = null;
            return;
        }
        Vector3 faceLocation = GetPlaceLocation();
        if (faceLocation == Vector3.zero)
        {
            if (previewTransform == null)
                return;
            Destroy(previewTransform.gameObject);
            previewTransform = null;
            return;
        }

        if (previewTransform == null)
        {
            previewTransform = currentItem.InstantiateItem(faceLocation, PlayerController.GetInstance().transform.rotation).transform;
            PreparePreviewItems();
            return;
        }
        Vector3 placeLocation = currentItem.GetPlaceLocation(faceLocation, PlayerController.GetInstance().transform.rotation.eulerAngles.y);
        Quaternion placeRotation;
        if (currentItem.CanSnapNoCheck())
            placeRotation = currentItem.GetSnapLocationNoCheck();
        else
            placeRotation = PlayerController.GetInstance().transform.rotation;
        previewTransform.SetPositionAndRotation(placeLocation, placeRotation);

        if (currentItem.CanPlaceNoCheck())
        {
            blueprintMaterial.SetColor("_GlowColor", blueprintCanPlaceColor);
        } else
        {
            blueprintMaterial.SetColor("_GlowColor", blueprintCannotPlaceColor);
        }
    }

    private void PreparePreviewItems()
    {
        PreparePreviewItem(previewTransform);

        for (int i = 0; i < previewTransform.childCount; i++)
        {
            PreparePreviewItem(previewTransform.GetChild(i));
        }
    }

    private void PreparePreviewItem(Transform item)
    {
        MeshRenderer renderer = item.GetComponent<MeshRenderer>();
        if(renderer != null)
            renderer.material = blueprintMaterial;

        foreach (Collider collider in item.GetComponents<Collider>())
        {
            collider.enabled = false;
        }
    }

    private void PlaceItem()
    {
        PlaceableItem placeableItem = GetCurrentItem();
        if (placeableItem == null)
            return;
        Vector3 placeLocation = GetPlaceLocation();
        if (placeLocation == Vector3.zero)
            return;
        if (!placeableItem.CanPlaceNoCheck())
            return;
        Quaternion placeRotation;
        if (placeableItem.CanSnapNoCheck())
            placeRotation = placeableItem.GetSnapLocationNoCheck();
        else
            placeRotation = PlayerController.GetInstance().transform.rotation;
        GroundItemManager.GetInstance().AddGroundItem(placeableItem.GetTypeID(), 1, placeLocation, placeRotation);
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
