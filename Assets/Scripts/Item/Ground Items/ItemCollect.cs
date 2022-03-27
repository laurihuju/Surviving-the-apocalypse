using UnityEngine;

public class ItemCollect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        GroundItem groundItem = GetComponent<GroundItem>();
        if (groundItem == null)
            return;

        GroundItemManager.GetInstance().UnregisterGroundItem(groundItem);

        Debug.Log("Ground item with id " + groundItem.GetTypeID() + " collected");

        //Item collected. Code collection later when inventory is coded. Get collected item id from groundItem by calling groundItem.GetTypeID().
    }
}
