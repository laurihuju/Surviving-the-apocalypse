using UnityEngine;

public class GroundItem : MonoBehaviour
{
    [SerializeField] private int typeID;
    [SerializeField] private int amount;

    public GroundItemData GenerateData()
    {
        return new GroundItemData(typeID, amount, transform.position, transform.rotation);
    }

    /// <summary>
    /// Returns the type ID of this ground item.
    /// </summary>
    /// <returns></returns>
    public int GetTypeID()
    {
        return typeID;
    }

    /// <summary>
    /// Sets the item amount of this ground item stack.
    /// </summary>
    /// <param name="amount"></param>
    public void SetAmount(int amount)
    {
        this.amount = amount;
    }

    /// <summary>
    /// Returns the item amount of this ground item stack.
    /// </summary>
    /// <returns></returns>
    public int GetAmount()
    {
        return amount;
    }
}
