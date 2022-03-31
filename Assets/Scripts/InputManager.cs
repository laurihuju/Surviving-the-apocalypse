using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    private InputActions input;

    private List<DisableCallback> disableCallbacks;

    public delegate void DisableCallback(byte disabledControls);

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        input = new InputActions();

        disableCallbacks = new List<DisableCallback>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EnableGameControls()
    {
        Cursor.lockState = CursorLockMode.Locked;
        input.Game.Enable();
        input.Menu.Disable();

        foreach(DisableCallback callback in disableCallbacks)
        {
            callback(1);
        }
    }

    public void EnableMenuControls()
    {
        Cursor.lockState = CursorLockMode.None;
        input.Game.Disable();
        input.Menu.Enable();

        foreach (DisableCallback callback in disableCallbacks)
        {
            callback(0);
        }
    }

    public void AddDisableCallback(DisableCallback callback)
    {
        disableCallbacks.Add(callback);
    }

    public InputActions GetInputActions()
    {
        return input;
    }

    public static InputManager GetInstance()
    {
        return instance;
    }

    private void OnEnable()
    {
        input.Enable();
    }
}
