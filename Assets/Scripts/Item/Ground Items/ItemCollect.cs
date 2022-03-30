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

        Inventory.GetInstance().AddItem(groundItem.GetTypeID(), groundItem.GetAmount());
        Destroy(gameObject);
    }
}
