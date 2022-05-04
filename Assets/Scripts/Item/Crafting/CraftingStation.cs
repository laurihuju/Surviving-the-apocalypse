using UnityEngine;

public class CraftingStation : MonoBehaviour
{
    [SerializeField] private CraftingRecipe[] recipes;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        for(int i = 0; i < recipes.Length; i++)
        {
            CraftingMenu.GetInstance().AddRecipe(recipes[i]);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        for (int i = 0; i < recipes.Length; i++)
        {
            CraftingMenu.GetInstance().RemoveRecipe(recipes[i]);
        }
    }
}
