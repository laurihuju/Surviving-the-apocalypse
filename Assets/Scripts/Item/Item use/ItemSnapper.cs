using UnityEngine;

public class ItemSnapper : MonoBehaviour
{
    [Tooltip("The ID of the snapper type.")]
    [SerializeField] private int snapperID;
    [SerializeField] private bool showBase = true;

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

    public bool ShowBase()
    {
        return showBase;
    }
}
