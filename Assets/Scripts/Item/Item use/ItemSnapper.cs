using UnityEngine;

public class ItemSnapper : MonoBehaviour
{
    [Tooltip("The ID of the snapper type.")]
    [SerializeField] private int snapperID;

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }

    public int GetID()
    {
        return snapperID;
    }
}
