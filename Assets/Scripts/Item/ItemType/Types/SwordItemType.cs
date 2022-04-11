using UnityEngine;

public class SwordItemType : UsableItem
{
    [Header("Animation")]
    [SerializeField] private Animator armAnimator;

    public override bool CanUse()
    {
        return true;
    }

    public override void OnCollect()
    {

    }

    public override void OnDeselect()
    {
        MeleeManager.GetInstance().SetItem(null);
    }

    public override void OnDrop()
    {

    }

    public override void OnSelect()
    {
        MeleeManager.GetInstance().SetItem(this);
    }

    public override void Use()
    {
        armAnimator.SetTrigger("Attack");
    }
}
