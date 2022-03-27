using UnityEngine;

public class WoodItemType : ItemType
{
    public override void OnCollect()
    {
        Debug.Log("Wood Collected");
    }

    public override void OnDrop()
    {
        Debug.Log("Wood dropped");
    }
}
