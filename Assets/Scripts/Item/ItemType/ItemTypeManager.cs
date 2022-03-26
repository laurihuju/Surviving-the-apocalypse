using UnityEngine;

public class ItemTypeManager : MonoBehaviour
{
    private static ItemTypeManager instance;

    [SerializeField] private ItemType[] types;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    /// <summary>
    /// Returns the registered item type that has the given ID. If item type didn't found, the function will return null.
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public ItemType GetItemType(int ID)
    {
        for(int i = 0; i < types.Length; i++)
        {
            if (types[i].GetTypeID() == ID)
                return types[i];
        }
        return null;
    }
}
