using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CraftingMenu : MonoBehaviour
{
    private static CraftingMenu instance;

    [Header("Slot")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotParent;
    [SerializeField] private Sprite selectedSlotCanCraftSprite;
    [SerializeField] private Sprite unselectedSlotCanCraftSprite;
    [SerializeField] private Sprite selectedSlotCannotCraftSprite;
    [SerializeField] private Sprite unselectedSlotCannotCraftSprite;

    [Header("Craft Button")]
    [SerializeField] private Button craftButton;

    [Header("Required Items Display")]
    [SerializeField] private GameObject requiredItemSlotPrefab;
    [SerializeField] private Transform requiredItemSlotsParent;

    [Header("Other")]
    [SerializeField] private GridLayoutGroup slotsLayout;
    [SerializeField] private List<CraftingRecipe> recipes;

    private List<CraftingSlot> slots;
    private int selectedSlot = 0;

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
        slots = new List<CraftingSlot>();
        for (int i = 0; i < recipes.Count; i++)
        {
            CraftingSlot slot = Instantiate(slotPrefab, slotParent).GetComponent<CraftingSlot>();
            slots.Add(slot);

            ItemType type = ItemTypeManager.GetInstance().GetItemType(recipes[i].GetCraftingType());
            if (type == null)
                return;
            slot.SetImage(type.GetSprite());
            slot.SetAmount(recipes[i].GetCraftingAmount());
        }

        bool canCraft = recipes[selectedSlot].CanCraft();
        slots[selectedSlot].SetSelection(GetSlotSprite(true, canCraft));
        UpdateCraftButtonSprites(canCraft);
        UpdateCraftButtonPosition();
        UpdateRequiredItemsDisplay(recipes[selectedSlot]);

        craftButton.onClick.AddListener(Craft);
    }

    public void AddRecipe(CraftingRecipe recipe)
    {
        recipes.Add(recipe);

        CraftingSlot slot = Instantiate(slotPrefab, slotParent).GetComponent<CraftingSlot>();
        slots.Add(slot);

        UpdateCraftButtonPosition();

        ItemType type = ItemTypeManager.GetInstance().GetItemType(recipe.GetCraftingType());
        if (type == null)
            return;
        slot.SetImage(type.GetSprite());
        slot.SetAmount(recipe.GetCraftingAmount());
        slot.SetSelection(GetSlotSprite(selectedSlot == recipes.IndexOf(recipe), recipe.CanCraft()));
    }

    public void RemoveRecipe(CraftingRecipe recipe)
    {
        int recipeIndex = recipes.IndexOf(recipe);
        CraftingSlot slotToDestroy = slots[recipeIndex];

        Destroy(slotToDestroy.gameObject);

        slots.Remove(slotToDestroy);
        recipes.Remove(recipe);

        UpdateCraftButtonPosition();

        if (selectedSlot != recipeIndex)
            return;
        selectedSlot = 0;
        bool canCraft = recipes[0].CanCraft();
        slots[0].SetSelection(GetSlotSprite(true, canCraft));

        UpdateCraftButtonSprites(canCraft);

        UpdateRequiredItemsDisplay(recipes[selectedSlot]);
    }

    public void UpdateCanCraftInSlots(int itemTypeUsedInRecipe)
    {
        for(int i = 0; i < slots.Count; i++)
        {
            if (!recipes[i].RequiresType(itemTypeUsedInRecipe))
                continue;

            bool canCraft = recipes[i].CanCraft();
            slots[i].SetSelection(GetSlotSprite(selectedSlot == i, canCraft));

            if(selectedSlot == i)
            {
                UpdateCraftButtonSprites(canCraft);
                UpdateRequiredItemsDisplay(recipes[selectedSlot]);
            }
        }
    }

    public void SetActiveSlot(CraftingSlot slot)
    {
        if (slot == slots[selectedSlot])
            return;

        slots[selectedSlot].SetSelection(GetSlotSprite(false, recipes[selectedSlot].CanCraft()));
        selectedSlot = slots.IndexOf(slot);
        bool canCraft = recipes[selectedSlot].CanCraft();
        slot.SetSelection(GetSlotSprite(true, canCraft));

        UpdateCraftButtonSprites(canCraft);

        UpdateRequiredItemsDisplay(recipes[selectedSlot]);
    }

    private Sprite GetSlotSprite(bool selected, bool canCraft)
    {
        if(selected && canCraft)
            return selectedSlotCanCraftSprite;
        if(!selected && canCraft)
            return unselectedSlotCanCraftSprite;
        if (selected)
            return selectedSlotCannotCraftSprite;
        return unselectedSlotCannotCraftSprite;
    }

    private void UpdateCraftButtonPosition()
    {
        int slotRowCount = GetSlotRowCount();
        craftButton.transform.position = new Vector3(craftButton.transform.position.x, slotsLayout.transform.position.y - slotRowCount * slotsLayout.cellSize.y - slotRowCount * slotsLayout.spacing.y, craftButton.transform.position.z);
    }

    private void UpdateCraftButtonSprites(bool canCraft)
    {
        craftButton.interactable = canCraft;
    }

    private int GetSlotRowCount()
    {
        RectTransform slotsLayoutTransform = slotsLayout.GetComponent<RectTransform>();
        int rowSlotCount = (int)((slotsLayoutTransform.sizeDelta.x + slotsLayout.spacing.x) / (slotsLayout.cellSize.x + slotsLayout.spacing.x));
        float fullRows = (float)slotsLayoutTransform.childCount / rowSlotCount;
        return slotsLayoutTransform.childCount % rowSlotCount == 0 ? (int)fullRows : (int)fullRows + 1;
    }

    private void UpdateRequiredItemsDisplay(CraftingRecipe recipe)
    {
        foreach (Transform child in requiredItemSlotsParent)
            Destroy(child.gameObject);

        for(int i = 0; i < recipe.GetRequireItemTypes().Length; i++)
        {
            RequiredItemSlot slot =  Instantiate(requiredItemSlotPrefab, requiredItemSlotsParent).GetComponent<RequiredItemSlot>();

            ItemType type = ItemTypeManager.GetInstance().GetItemType(recipe.GetRequireItemTypes()[i]);
            if (type == null)
                continue;

            slot.SetImage(type.GetSprite());
            slot.SetAmount(recipe.GetRequireItemAmounts()[i]);

            slot.SetSlotImage(Inventory.GetInstance().GetItemAmount(recipe.GetRequireItemTypes()[i]) < recipe.GetRequireItemAmounts()[i] ? unselectedSlotCannotCraftSprite : unselectedSlotCanCraftSprite);
        }
    }

    private void Craft()
    {
        for (int i = 0; i < recipes[selectedSlot].GetRequireItemTypes().Length; i++)
        {
            int removedAmount = Inventory.GetInstance().RemoveItem(recipes[selectedSlot].GetRequireItemTypes()[i], recipes[selectedSlot].GetRequireItemAmounts()[i]);
            if(removedAmount < recipes[selectedSlot].GetRequireItemAmounts()[i])
            {
                Inventory.GetInstance().AddItem(recipes[selectedSlot].GetRequireItemTypes()[i], removedAmount);
                for(int j = 0; j < i; j++)
                {
                    Inventory.GetInstance().AddItem(recipes[selectedSlot].GetRequireItemTypes()[j], recipes[selectedSlot].GetRequireItemAmounts()[j]);
                }
                return;
            }
        }
        Inventory.GetInstance().AddItem(recipes[selectedSlot].GetCraftingType(), recipes[selectedSlot].GetCraftingAmount());

        bool canCraftAnymore = recipes[selectedSlot].CanCraft();
        slots[selectedSlot].SetSelection(GetSlotSprite(true, canCraftAnymore));
        UpdateCraftButtonSprites(canCraftAnymore);
        UpdateRequiredItemsDisplay(recipes[selectedSlot]);
    }

    public static CraftingMenu GetInstance()
    {
        return instance;
    }
}
