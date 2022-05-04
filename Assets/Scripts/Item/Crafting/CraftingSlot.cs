using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CraftingSlot : MonoBehaviour, IPointerClickHandler
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

    public void SetSelection(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CraftingMenu.GetInstance().SetActiveSlot(this);
    }
}
