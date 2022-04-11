using UnityEngine;

public class MeleeItem : MonoBehaviour
{
    [SerializeField] private GameObject itemGameObject;

    public GameObject GetGameObject()
    {
        return itemGameObject;
    }
}
