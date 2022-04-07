using UnityEngine;

public class HotBar : MonoBehaviour
{
    private static HotBar instance;

    [Header("Slots")]
    [Tooltip("The inventory slot index of the first hotbar slot")]
    [SerializeField] private int firstSlot;
    [Tooltip("The inventory slot index of the last hotbar slot")]
    [SerializeField] private int lastSlot;

    [Header("Slot scale")]
    [SerializeField] private float normalSlotSize;
    [SerializeField] private float activeSlotSize;

    private int activeSlot;
    private ItemStack selectedStack;

    private bool shiftPressed = false;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        InputManager.GetInstance().GetInputActions().Game.Scroll.performed += context => Scroll(context.ReadValue<float>());
        InputManager.GetInstance().GetInputActions().Game.Shift.performed += context => shiftPressed = true;
        InputManager.GetInstance().GetInputActions().Game.Shift.canceled += context => shiftPressed = false;

        activeSlot = firstSlot;
        Inventory.GetInstance().GetSlot(firstSlot).GetComponent<RectTransform>().sizeDelta = new Vector2(activeSlotSize, activeSlotSize);
    }

    private void Update()
    {
        if (Inventory.GetInstance().GetStack(activeSlot) != selectedStack)
        {
            if (selectedStack != null)
            {
                ItemType selectedType = ItemTypeManager.GetInstance().GetItemType(selectedStack.GetTypeID());
                if (selectedType != null)
                {
                    selectedType.OnDeselect();
                }
            }
            selectedStack = Inventory.GetInstance().GetStack(activeSlot);
            if (selectedStack != null)
            {
                ItemType selectedType = ItemTypeManager.GetInstance().GetItemType(selectedStack.GetTypeID());
                if (selectedType != null)
                {
                    selectedType.OnSelect();
                }
            }
        }
    }

    private void Scroll(float direction)
    {
        if (shiftPressed)
            return;

        if(direction < 0)
        {
            activeSlot++;
            if (activeSlot > lastSlot)
            {
                activeSlot = lastSlot;
                return;
            }
            Inventory.GetInstance().GetSlot(activeSlot - 1).GetComponent<RectTransform>().sizeDelta = new Vector2(normalSlotSize, normalSlotSize);
        } else if (direction > 0)
        {
            activeSlot--;
            if (activeSlot < firstSlot)
            {
                activeSlot = firstSlot;
                return;
            }
            Inventory.GetInstance().GetSlot(activeSlot + 1).GetComponent<RectTransform>().sizeDelta = new Vector2(normalSlotSize, normalSlotSize);
        } else if (direction == 0)
        {
            return;
        }

        Inventory.GetInstance().GetSlot(activeSlot).GetComponent<RectTransform>().sizeDelta = new Vector2(activeSlotSize, activeSlotSize);
    }

    /// <summary>
    /// Returns the inventory slot index of the active slot.
    /// </summary>
    /// <returns></returns>
    public int GetActiveSlot()
    {
        return activeSlot;
    }

    public static HotBar GetInstance()
    {
        return instance;
    }
}
