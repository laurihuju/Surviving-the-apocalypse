/// <summary>
/// Class for storing item stack's data.
/// </summary>
[System.Serializable]
public class ItemStack
{
    private int typeID;
    private int amount;

    public ItemStack(int typeID, int amount)
    {
        this.typeID = typeID;
        this.amount = amount;
    }

    public ItemStack(int typeID)
    {
        this.typeID = typeID;
        this.amount = 0;
    }

    /// <summary>
    /// Returns the type ID of this item stack.
    /// </summary>
    /// <returns></returns>
    public int GetTypeID()
    {
        return typeID;
    }

    /// <summary>
    /// Sets the item amount of this item stack.
    /// </summary>
    /// <param name="newAmount"></param>
    public void SetAmount(int newAmount)
    {
        amount = newAmount;
    }

    /// <summary>
    /// Returns the item amount of this item stack.
    /// </summary>
    /// <returns></returns>
    public int GetAmount()
    {
        return amount;
    }
}
