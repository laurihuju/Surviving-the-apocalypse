using UnityEngine;
using Menu;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;

    [SerializeField] private Rigidbody rigidBody;

    [Header("Movement")]
    [SerializeField] [Tooltip("Maximum walking speed")] private float walkingSpeed;
    [SerializeField] [Tooltip("Maximum running speed")] private float runningSpeed;
    [SerializeField] private float speedChange;
    [SerializeField] private float jumpSpeedChange;
    [SerializeField] [Tooltip("The speed of the camera movement.")] private float cameraSpeed;
    [SerializeField] private float cameraMinRotation;
    [SerializeField] private float cameraMaxRotation;

    [Header("Jump")]
    [SerializeField] [Tooltip("The force applied on jump")] private float jumpForce;
    [SerializeField] [Tooltip("The point where ground check is done")] private GameObject groundCheckPoint;
    [SerializeField] [Tooltip("Distance from the Ground Check Point to be grounded")] private float groundedDistance;

    [Header("Physics")]
    [SerializeField] [Tooltip("The gravity to set to the physics system")] private float gravity;
    [SerializeField] private Collider playerCollider;

    [Header("Animation")]
    [SerializeField] private Animator armsAnim;

    [Header("ZombieSound")]
    [SerializeField] private float zombieSoundSendingDelay = 0.5f;
    [SerializeField] private float zombieSoundVolume = 1;

    [Header("Other")]
    [SerializeField] private HealthManager healthManager;

    private Vector2 movement;
    private Vector2 mouseMovement;
    private Vector3 previousVelocity = Vector3.zero;
    private bool shiftPressed = false;
    private bool isGrounded = false;

    private float xRotation = 0;

    private float nextTimeToSendZombieSound = 0;

    public Collider PlayerCollider { get => playerCollider;}
    public HealthManager HealthManager { get => healthManager;}
    public bool ShiftPressed { get => shiftPressed;}

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

        InputManager.GetInstance().GetInputActions().Game.Shift.performed += context => shiftPressed = true;
        InputManager.GetInstance().GetInputActions().Game.Shift.canceled += context => shiftPressed = false;

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
        if (!isGrounded)
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
        {
            armsAnim.SetBool("Walking", false);
        } else
        {
            armsAnim.SetBool("Walking", true);
            if (Time.time > nextTimeToSendZombieSound)
            {
                ZombieManager.GetInstance().SendSoundToZombies(zombieSoundVolume, transform.position);
                nextTimeToSendZombieSound = Time.time + zombieSoundSendingDelay;
            }
        }
        HandleCameraRotation();
    }

    private void FixedUpdate()
    {
        isGrounded = IsGrounded();
        HandleMovement();
    }

    private void HandleMovement()
    {
        float speedToUse = ((shiftPressed/* && stamina_check!*/) ? runningSpeed : walkingSpeed);
        Vector3 newVelocity = Vector3.Lerp(previousVelocity, transform.forward * movement.y * speedToUse + transform.right * movement.x * speedToUse, isGrounded ? speedChange : jumpSpeedChange);
        newVelocity[1] = rigidBody.velocity.y;
        rigidBody.velocity = newVelocity;

        previousVelocity = newVelocity;
    }

    private void HandleCameraRotation()
    {
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
