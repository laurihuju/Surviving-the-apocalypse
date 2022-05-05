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

        int addedAmount = Inventory.GetInstance().AddItem(groundItem.GetTypeID(), groundItem.GetAmount());
        if(addedAmount < groundItem.GetAmount())
        {
            groundItem.SetAmount(groundItem.GetAmount() - addedAmount);
            return;
        }
        GroundItemManager.GetInstance().UnregisterGroundItem(groundItem);
        Destroy(gameObject);
    }
}
