using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUISlot : MonoBehaviour
{
    [SerializeField] private Image slotImage;
    [SerializeField] private TextMeshProUGUI slotText;

    [SerializeField] private int slotIndex;

    /// <summary>
    /// Sets new image to the slot image.
    /// </summary>
    /// <param name="image"></param>
    public void SetSlotImage(Sprite image)
    {
        if(image == null)
        {
            slotImage.gameObject.SetActive(false);
            return;
        }

        if(!slotImage.gameObject.activeSelf)
            slotImage.gameObject.SetActive(true);
        slotImage.sprite = image;
    }

    /// <summary>
    /// Changes the text in the slot amount text.
    /// </summary>
    /// <param name="amount"></param>
    public void SetSlotAmountText(int amount)
    {
        if(amount <= 0)
        {
            slotText.gameObject.SetActive(false);
            return;
        }

        if (!slotText.gameObject.activeSelf)
            slotText.gameObject.SetActive(true);
        slotText.text = amount.ToString();
    }

    /// <summary>
    /// Returns the index of this slot.
    /// </summary>
    /// <returns></returns>
    public int GetSlotIndex()
    {
        return slotIndex;
    }
}
