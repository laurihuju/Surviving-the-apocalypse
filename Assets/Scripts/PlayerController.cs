using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputActions input;

    [SerializeField] private Rigidbody rigidBody;

    [Header("Movement")]
    [Tooltip("Maximum speed")]
    [SerializeField] private float speed;
    [Tooltip("The speed of the camera movement.")]
    [SerializeField] private float cameraSpeed;

    [Header("Jump")]
    [Tooltip("The force applied on jump")]
    [SerializeField] private float jumpForce;
    [Tooltip("The point where ground check is done")]
    [SerializeField] private GameObject groundCheckPoint;
    [Tooltip("Distance from the Ground Check Point to be grounded")]
    [SerializeField] private float groundedDistance;

    [Header("Physics")]
    [Tooltip("The gravity to set to the physics system")]
    [SerializeField] private float gravity;

    private Vector2 movement;
    private Vector2 mouseMovement;

    private void Awake()
    {
        input = new InputActions();

        input.Game.Movement.performed += context => movement = context.ReadValue<Vector2>();
        input.Game.Movement.canceled += context => movement = context.ReadValue<Vector2>();

        input.Game.View.performed += context => mouseMovement = context.ReadValue<Vector2>();

        input.Game.Jump.performed += _ => Jump();

        Cursor.lockState = CursorLockMode.Locked;

        Physics.gravity = new Vector3(0, gravity, 0);
    }

    private void Jump()
    {
        if (!IsGrounded())
            return;
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce, rigidBody.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(groundCheckPoint.transform.position, Vector3.down, groundedDistance);
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
