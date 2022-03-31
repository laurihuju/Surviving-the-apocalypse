/// <summary>
/// Consumamble item type base class that has some extra functions related to consuming item.
/// </summary>
public abstract class UsableItem : ItemType
{
    /// <summary>
    /// Returns whether this type of item can be uset at the movement.
    /// </summary>
    /// <returns></returns>
    public abstract bool CanUse();

    /// <summary>
    /// Called when the item is used.
    /// </summary>
    public abstract void Use();
}
