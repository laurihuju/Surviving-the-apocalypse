using UnityEngine;
using Menu;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;

    [SerializeField] private Rigidbody rigidBody;

    [Header("Movement")]
    [Tooltip("Maximum speed")]
    [SerializeField] private float speed;
    [Tooltip("The speed of the camera movement.")]
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float cameraMinRotation;
    [SerializeField] private float cameraMaxRotation;

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

    [Header("Animation")]
    [SerializeField] private Animator armsAnim;

    private Vector2 movement;
    private Vector2 mouseMovement;

    private float xRotation = 0;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        Physics.gravity = new Vector3(0, gravity, 0);
    }

    private void Start()
    {
        InputManager.GetInstance().GetInputActions().Game.Movement.performed += context => movement = context.ReadValue<Vector2>();
        InputManager.GetInstance().GetInputActions().Game.Movement.canceled += context => movement = context.ReadValue<Vector2>();

        InputManager.GetInstance().GetInputActions().Game.View.performed += context => mouseMovement = context.ReadValue<Vector2>();

        InputManager.GetInstance().GetInputActions().Game.Jump.performed += _ => Jump();

        InputManager.GetInstance().GetInputActions().Game.InventoryOpen.performed += _ => MenuSystem.GetInstance().ShowPage(0);
        InputManager.GetInstance().GetInputActions().Menu.Exit.performed += _ => MenuSystem.GetInstance().Hide();

        InputManager.GetInstance().AddDisableCallback(disabledControls => {
            if (disabledControls != 0)
                return;
            movement = Vector3.zero;
            mouseMovement = Vector3.zero;
        });
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
        if (movement.magnitude == 0)
            armsAnim.SetBool("Walking", false);
        else
            armsAnim.SetBool("Walking", true);

        Vector3 newVelocity = transform.forward * movement.y * speed + transform.right * movement.x * speed;
        newVelocity[1] = rigidBody.velocity.y;
        rigidBody.velocity = newVelocity;

        transform.Rotate(new Vector3(0, mouseMovement.x * cameraSpeed, 0));

        xRotation += mouseMovement.y * -cameraSpeed;
        xRotation = Mathf.Clamp(xRotation, cameraMinRotation, cameraMaxRotation);
        Camera.main.transform.localEulerAngles = new Vector3(xRotation, 0, 0);
    }

    public static PlayerController GetInstance()
    {
        return instance;
    }
}
