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
    private GameObject previewBase;
    private int previewItemID = -1;

    private void Start()
    {
        InputManager.GetInstance().GetInputActions().Game.Use.performed += _ => PlaceItem();

        Inventory.GetInstance().AddItem(27658, 1);
        Inventory.GetInstance().AddItem(5738, 1);
        Inventory.GetInstance().AddItem(5739, 1);
    }

    private void Update()
    {
        PlaceableItem currentItem = GetCurrentItem();
        if (currentItem == null)
        {
            if (previewTransform != null)
            {
                Destroy(previewTransform.gameObject);
                previewTransform = null;
            }
            if (previewBase != null)
            {
                Destroy(previewBase.gameObject);
                previewBase = null;
            }
            return;
        }
        Vector3 faceLocation = GetPlaceLocation();
        if (faceLocation == Vector3.zero)
        {
            if (previewTransform != null)
            {
                Destroy(previewTransform.gameObject);
                previewTransform = null;
            }
            if (previewBase != null)
            {
                Destroy(previewBase.gameObject);
                previewBase = null;
            }
            return;
        }

        if (previewTransform == null || previewItemID != currentItem.GetPlaceItemID())
        {
            if (previewTransform != null)
            {
                Destroy(previewTransform.gameObject);
                previewTransform = null;
            }
            if (previewBase != null)
            {
                Destroy(previewBase.gameObject);
                previewBase = null;
            }
            previewTransform = currentItem.PlaceItem(faceLocation, PlayerController.GetInstance().transform.rotation).transform;
            PreparePreviewItems();
            previewItemID = currentItem.GetPlaceItemID();
            return;
        }
        Vector3 placeLocation = currentItem.GetPlaceLocation(faceLocation, PlayerController.GetInstance().transform.rotation.eulerAngles.y);
        Quaternion placeRotation;
        if (currentItem.CanSnapNoCheck())
        {
            placeRotation = currentItem.GetSnapLocationNoCheck();
            if(previewBase != null)
                previewBase.SetActive(false);
        }
        else
        {
            placeRotation = PlayerController.GetInstance().transform.rotation;
            if(previewBase != null)
                previewBase.SetActive(true);
        }
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

        if (item.CompareTag("ItemBase"))
            previewBase = item.gameObject;

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
        bool showBase;
        if (placeableItem.CanSnapNoCheck())
        {
            placeRotation = placeableItem.GetSnapLocationNoCheck();
            showBase = false;
        } else{
            placeRotation = PlayerController.GetInstance().transform.rotation;
            showBase = true;
        }
        GroundItemManager.GetInstance().PlaceItem(placeableItem, placeLocation, placeRotation, showBase);
        placeableItem.OnPlace(HotBar.GetInstance().GetActiveSlot(), false);
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
