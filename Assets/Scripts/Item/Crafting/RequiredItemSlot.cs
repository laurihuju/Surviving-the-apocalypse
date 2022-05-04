using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RequiredItemSlot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI amountText;

    [SerializeField] private Image image;

    public void SetImage(Sprite image)
    {
        itemImage.sprite = image;
    }

    public void SetAmount(int amount)
    {
        amountText.text = "" + amount;
    }

    public void SetSlotImage(Sprite sprite)
    {
        image.sprite = sprite;
    }
}
