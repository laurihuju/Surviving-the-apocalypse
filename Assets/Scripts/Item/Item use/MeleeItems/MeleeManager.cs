using UnityEngine;

public class MeleeManager : MonoBehaviour
{
    private static MeleeManager instance;

    [SerializeField] private Animator armAnimator;

    private MeleeItem currentItem;
    private MeleeItem previousItem;

    private bool isTakingItem = false;

    private void Start()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void SetItem(ItemType type)
    {
        bool alreadyTakingItem = isTakingItem;
        isTakingItem = true;
        if (type == null)
        {
            if(currentItem != null)
            {
                if (!alreadyTakingItem)
                {
                    armAnimator.SetTrigger("TakeItem");
                    previousItem = currentItem;
                }
                currentItem = null;
            }
            return;
        }
        MeleeItem item = type.GetComponent<MeleeItem>();
        if(item == null)
        {
            if (currentItem != null)
            {
                if (!alreadyTakingItem)
                {
                    armAnimator.SetTrigger("TakeItem");
                    previousItem = currentItem;
                }
                currentItem = null;
            }
            return;
        }

        if (!alreadyTakingItem)
        {
            armAnimator.SetTrigger("TakeItem");
            previousItem = currentItem;
        }
        currentItem = item;
    }

    public void AnimationChangeItem()
    {
        isTakingItem = false;
        if (previousItem != null)
            previousItem.GetGameObject().SetActive(false);

        if (currentItem != null)
        {
            currentItem.GetGameObject().SetActive(true);
            armAnimator.SetBool("HoldingItem", true);
            return;
        }
        armAnimator.SetBool("HoldingItem", false);
    }

    public static MeleeManager GetInstance()
    {
        return instance;
    }
}
