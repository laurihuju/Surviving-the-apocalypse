using UnityEngine;

public class MenuAnimationListener : MonoBehaviour
{
    [SerializeField] private string methodToCallOnHide;

    public void HideAnimationEnded()
    {
        if (methodToCallOnHide.Length == 0)
            return;
        BroadcastMessage(methodToCallOnHide);
    }
}
