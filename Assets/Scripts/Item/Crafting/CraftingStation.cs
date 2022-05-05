using UnityEngine;

public class CraftingStation : MonoBehaviour
{
    [SerializeField] private CraftingRecipe[] recipes;

    [Header("Hint")]
    [SerializeField] private string hintText;

    private Hint currentHint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        for(int i = 0; i < recipes.Length; i++)
        {
            CraftingMenu.GetInstance().AddRecipe(recipes[i]);
        }

        if(currentHint == null)
            currentHint = HintSystem.GetInstance().ShowHint(hintText);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        for (int i = 0; i < recipes.Length; i++)
        {
            CraftingMenu.GetInstance().RemoveRecipe(recipes[i]);
        }
        HintSystem.GetInstance().RemoveHint(currentHint);
        currentHint = null;
    }
}
