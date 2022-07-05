using UnityEngine;
using UnityEngine.UI;

public class BuildingTool : PlaceableItem
{
    private static BuildingTool instance;

    [Header("Building Tool")]
    [Tooltip("The building tool menu GameObject.")]
    [SerializeField] private GameObject buildingToolMenu;

    [Tooltip("The slot images where selection items will show.")]
    [SerializeField] private Image[] selectionSlots;

    [Tooltip("The item types to use with the building tool.")]
    [SerializeField] private PlaceableItem[] buildingItemTypes;
    [SerializeField] private CraftingRecipe[] buildingItemRecipes;

    [Header("Required Items Display")]
    [SerializeField] private GameObject requiredItemSlotPrefab;
    [SerializeField] private Transform requiredItemSlotsParent;
    [SerializeField] private Sprite canCraftRequiredItemSlotSprite;
    [SerializeField] private Sprite cannotCraftRequiredItemSlotSprite;

    private int selectedItem = 0;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        InputManager.GetInstance().GetInputActions().Game.Scroll.performed += context => MoveSelection(context.ReadValue<float>());

        UpdateUI();
    }

    private void MoveSelection(float direction)
    {
        if (!buildingToolMenu.activeSelf)
            return;
        if (!PlayerController.GetInstance().ShiftPressed)
            return;

        if(direction < 0)
        {
            selectedItem--;
            if(selectedItem < 0)
            {
                selectedItem = 0;
                return;
            }
        } else if (direction > 0)
        {
            selectedItem++;
            if(selectedItem >= buildingItemTypes.Length)
            {
                selectedItem = buildingItemTypes.Length - 1;
                return;
            }
        } else
        {
            return;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        int middleSlot = selectionSlots.Length / 2;

        selectionSlots[middleSlot].gameObject.SetActive(true);
        selectionSlots[middleSlot].sprite = buildingItemTypes[selectedItem].GetSprite();
        for(int i = 0; i < middleSlot; i++)
        {
            int slotItem = selectedItem - (middleSlot - i);
            if (slotItem < 0)
            {
                selectionSlots[i].gameObject.SetActive(false);
                continue;
            }
            selectionSlots[i].gameObject.SetActive(true);
            selectionSlots[i].sprite = buildingItemTypes[slotItem].GetSprite();
        }
        for (int i = middleSlot + 1; i < selectionSlots.Length; i++)
        {
            int slotItem = selectedItem + i - middleSlot;
            if (slotItem >= buildingItemTypes.Length)
            {
                selectionSlots[i].gameObject.SetActive(false);
                continue;
            }
            selectionSlots[i].gameObject.SetActive(true);
            selectionSlots[i].sprite = buildingItemTypes[slotItem].GetSprite();
        }

        UpdateRequiredItemsDisplay();
    }

    public void UpdateRequiredItemsDisplay()
    {
        if (!buildingToolMenu.activeSelf)
            return;

        foreach (Transform child in requiredItemSlotsParent)
            Destroy(child.gameObject);

        for (int i = 0; i < buildingItemRecipes[selectedItem].GetRequireItemTypes().Length; i++)
        {
            RequiredItemSlot slot = Instantiate(requiredItemSlotPrefab, requiredItemSlotsParent).GetComponent<RequiredItemSlot>();

            ItemType type = ItemTypeManager.GetInstance().GetItemType(buildingItemRecipes[selectedItem].GetRequireItemTypes()[i]);
            if (type == null)
                continue;

            slot.SetImage(type.GetSprite());
            slot.SetAmount(buildingItemRecipes[selectedItem].GetRequireItemAmounts()[i]);

            slot.SetSlotImage(Inventory.GetInstance().GetItemAmount(buildingItemRecipes[selectedItem].GetRequireItemTypes()[i]) < buildingItemRecipes[selectedItem].GetRequireItemAmounts()[i] ? cannotCraftRequiredItemSlotSprite : canCraftRequiredItemSlotSprite);
        }
    }

    public override void OnCollect()
    {

    }

    public override void OnDeselect()
    {
        buildingToolMenu.SetActive(false);
    }

    public override void OnDrop()
    {

    }

    public override void OnSelect()
    {
        buildingToolMenu.SetActive(true);
        UpdateRequiredItemsDisplay();
    }

    public override bool CanPlaceNoCheck()
    {
        return buildingItemTypes[selectedItem].CanPlaceNoCheck() && buildingItemRecipes[selectedItem].CanCraft();
    }

    public override Vector3 GetPlaceLocation(Vector3 location, float yRotation)
    {
        return buildingItemTypes[selectedItem].GetPlaceLocation(location, yRotation);
    }

    public override bool CanSnapNoCheck()
    {
        return buildingItemTypes[selectedItem].CanSnapNoCheck();
    }

    public override Quaternion GetSnapLocationNoCheck()
    {
        return buildingItemTypes[selectedItem].GetSnapLocationNoCheck();
    }

    public override GameObject PlaceItem(Vector3 position, Quaternion rotation)
    {
        return buildingItemTypes[selectedItem].PlaceItem(position, rotation);
    }

    public override int GetPlaceItemID()
    {
        return buildingItemTypes[selectedItem].GetPlaceItemID();
    }

    public override void OnPlace(int slot, bool alreadyRemoved)
    {
        for(int i = 0; i < buildingItemRecipes[selectedItem].GetRequireItemTypes().Length; i++)
        {
            if (i >= buildingItemRecipes[selectedItem].GetRequireItemAmounts().Length)
                break;
            Inventory.GetInstance().RemoveItem(buildingItemRecipes[selectedItem].GetRequireItemTypes()[i], buildingItemRecipes[selectedItem].GetRequireItemAmounts()[i]);
        }
        buildingItemTypes[selectedItem].OnPlace(slot, true);
    }

    public static BuildingTool GetInstance()
    {
        return instance;
    }
}
