using System.Collections;
using UnityEngine;

public class SwordItemType : UsableItem
{
    [Header("Animation")]
    [SerializeField] protected Animator armAnimator;

    [Header("Sword")]
    [SerializeField] protected Transform damagePoint;
    [SerializeField] protected float damageDistance;
    [SerializeField] protected float damageTime;
    [SerializeField] protected float damage;
    [SerializeField] protected float cooldown;

    protected float nextAttackTime = 0;

    public override bool CanUse()
    {
        return Time.time > nextAttackTime;
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
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        armAnimator.SetTrigger("Attack");

        nextAttackTime = Time.time + cooldown;

        yield return new WaitForSeconds(damageTime);

        Collider[] targets = Physics.OverlapSphere(damagePoint.position, damageDistance);
        for(int i = 0; i < targets.Length; i++)
        {
            if (!targets[i].CompareTag("Enemy"))
                continue;

            HealthManager health = targets[i].GetComponent<HealthManager>();
            if (health == null)
                continue;

            health.ChangeHealth(-damage);
        }
    }
}
