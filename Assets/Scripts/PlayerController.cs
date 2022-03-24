using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputActions input;

    private void Awake()
    {
        input = new InputActions();

        input.Game.Movement.performed += context => Move(context.ReadValue<Vector2>());
        input.Game.Movement.canceled += context => Move(context.ReadValue<Vector2>());
    }

    private void Move(Vector2 direction)
    {

    }

    private void OnEnable()
    {
        input.Enable();
    }
}
