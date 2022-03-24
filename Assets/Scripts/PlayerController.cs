using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputActions input;

    [SerializeField] private Rigidbody rigidBody;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float cameraSpeed;

    private Vector2 movement;
    private Vector2 mouseMovement;

    private void Awake()
    {
        input = new InputActions();

        input.Game.Movement.performed += context => movement = context.ReadValue<Vector2>();
        input.Game.Movement.canceled += context => movement = context.ReadValue<Vector2>();

        input.Game.View.performed += context => mouseMovement = context.ReadValue<Vector2>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Move(Vector2 direction)
    {
        movement = direction;
    }

    private void Update()
    {
        Vector3 newVelocity = transform.forward * movement.y * speed + transform.right * movement.x * speed;
        newVelocity[1] = rigidBody.velocity.y;
        rigidBody.velocity = newVelocity;

        transform.Rotate(new Vector3(0, mouseMovement.x * cameraSpeed, 0));
        Camera.main.transform.Rotate(new Vector3(mouseMovement.y * -cameraSpeed, 0, 0));
    }

    private void OnEnable()
    {
        input.Enable();
    }
}
