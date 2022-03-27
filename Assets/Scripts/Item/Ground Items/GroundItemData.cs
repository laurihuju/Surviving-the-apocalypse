using UnityEngine;

[System.Serializable]
public class GroundItemData : ItemStack
{
    private float posX;
    private float posY;
    private float posZ;

    private float rotX;
    private float rotY;
    private float rotZ;
    private float rotW;

    public GroundItemData(int typeID, int amount, Vector3 position, Quaternion rotation) : base(typeID, amount)
    {
        posX = position.x;
        posY = position.y;
        posZ = position.z;

        rotX = rotation.x;
        rotY = rotation.y;
        rotZ = rotation.z;
        rotW = rotation.w;
    }

    /// <summary>
    /// Sets the position of this ground item data.
    /// </summary>
    /// <param name="position"></param>
    public void SetPosition(Vector3 position)
    {
        posX = position.x;
        posY = position.y;
        posZ = position.z;
    }

    /// <summary>
    /// Returns the position of this ground item data.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPosition()
    {
        return new Vector3(posX, posY, posZ);
    }

    /// <summary>
    /// Sets the rotation of this ground item data.
    /// </summary>
    /// <param name="rotation"></param>
    public void SetRotation(Quaternion rotation)
    {
        rotX = rotation.x;
        rotY = rotation.y;
        rotZ = rotation.z;
        rotW = rotation.w;
    }

    /// <summary>
    /// Returns the rotation of this ground item data.
    /// </summary>
    /// <returns></returns>
    public Quaternion GetRotation()
    {
        return new Quaternion(rotX, rotY, rotZ, rotW);
    }
}
