using UnityEngine;

[System.Serializable]
public class CraftingRecipe
{
    [Tooltip("The item types to require for recipe use.")]
    [SerializeField] private int[] requireItemTypes;
    [Tooltip("The item amounts of Item Types in the other array to require for recipe use.")]
    [SerializeField] private int[] requireItemAmounts;

    [SerializeField] private int craftingType;
    [SerializeField] private int craftingAmount;

    public bool CanCraft()
    {
        for(int i = 0; i < requireItemTypes.Length; i++)
        {
            if (i >= requireItemAmounts.Length)
                break;
            if (Inventory.GetInstance().GetItemAmount(requireItemTypes[i]) < requireItemAmounts[i])
                return false;
        }
        return true;
    }

    public bool RequiresType(int typeID)
    {
        for(int i = 0; i < requireItemTypes.Length; i++)
        {
            if (requireItemTypes[i] == typeID)
                return true;
        }
        return false;
    }

    public int[] GetRequireItemTypes()
    {
        return requireItemTypes;
    }

    public int[] GetRequireItemAmounts()
    {
        return requireItemAmounts;
    }

    public int GetCraftingType()
    {
        return craftingType;
    }

    public int GetCraftingAmount()
    {
        return craftingAmount;
    }
}
