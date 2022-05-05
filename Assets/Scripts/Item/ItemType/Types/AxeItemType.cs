using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AxeItemType : SwordItemType
{
    [Header("Axe")]
    [SerializeField] private float chopDistance;
    [SerializeField] private LayerMask treeLayers;
    [SerializeField] private float treeDamage;
    [SerializeField] private float treeHP;
    [SerializeField] private float treeHealSpeed;

    [Header("Tree Health Bar")]
    [SerializeField] private Slider treeHealthBar;
    [SerializeField] private float treeHealthBarLerpSpeed;

    private float currentTreeDamage;
    private TreeInstance currentTree;

    private void Update()
    {
        if(currentTreeDamage > 0)
        {
            treeHealthBar.gameObject.SetActive(true);
            treeHealthBar.value = Mathf.Lerp(treeHealthBar.value, (treeHP - currentTreeDamage) / treeHP, treeHealthBarLerpSpeed);
        } else
        {
            treeHealthBar.gameObject.SetActive(false);
            treeHealthBar.value = 1;
        }
        HealTree();
    }

    public override bool CanUse()
    {
        return Time.time > nextAttackTime;
    }

    public override void Use()
    {
        StartCoroutine(UseCoroutine());
    }

    private IEnumerator UseCoroutine()
    {
        armAnimator.SetTrigger("Attack");

        nextAttackTime = Time.time + cooldown;

        yield return new WaitForSeconds(damageTime);

        //Chop trees
        Collider[] chopTargets = Physics.OverlapSphere(damagePoint.position, chopDistance, treeLayers);
        for(int i = 0; i < chopTargets.Length; i++)
        {
            if (!(chopTargets[i] is TerrainCollider))
                continue;
            Terrain terrain = chopTargets[i].GetComponent<Terrain>();
            TreeInstance chopTree = TreeChopping.GetInstance().GetTreeNearPosition(terrain, damagePoint.position);

            if (DamageTree(chopTree))
                TreeChopping.GetInstance().ChopTree(terrain, chopTree.position);
            break;
        }

        //Damage enemies
        Collider[] attackTargets = Physics.OverlapSphere(damagePoint.position, damageDistance);
        for (int i = 0; i < attackTargets.Length; i++)
        {
            if (attackTargets[i].CompareTag("Enemy"))
            {
                HealthManager health = attackTargets[i].GetComponent<HealthManager>();
                if (health == null)
                    continue;

                health.ChangeHealth(-damage);
            }
        }
    }

    private bool DamageTree(TreeInstance tree)
    {
        if (tree.position == Vector3.zero)
        {
            currentTreeDamage = 0;
            return false;
        }
        if(currentTree.position != tree.position)
        {
            currentTree = tree;
            currentTreeDamage = 0;
        }

        if(currentTreeDamage + treeDamage >= treeHP)
        {
            currentTreeDamage = 0;
            return true;
        }

        currentTreeDamage += treeDamage;
        return false;
    }

    private void HealTree()
    {
        if (currentTreeDamage == 0)
            return;

        if (currentTreeDamage - treeHealSpeed * Time.deltaTime <= 0)
        {
            currentTreeDamage = 0;
            return;
        }
        currentTreeDamage -= treeHealSpeed * Time.deltaTime;
    }
}
