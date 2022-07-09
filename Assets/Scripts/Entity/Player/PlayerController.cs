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

    private Vector2 movement; //Stores the player's current movement imput
    private Vector2 mouseMovement; //Stores the current mouse movement imput
    private Vector3 previousVelocity = Vector3.zero; //Stores the velocity from the previous running of the HandleMovement() method
    private bool shiftPressed = false; //Stores if the shift is pressed currently
    private bool isGrounded = false; //Stores the player's current grounded status

    private float xRotation = 0;

    private float nextTimeToSendZombieSound = 0;

    public Collider PlayerCollider { get => playerCollider;}
    public HealthManager HealthManager { get => healthManager;}
    public bool ShiftPressed { get => shiftPressed;}

    private void Awake()
    {
        //Set the singleton instance
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        //Set the physics gravity value
        Physics.gravity = new Vector3(0, gravity, 0);
    }

    private void Start()
    {
        //Register input actions
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

    /// <summary>
    /// Called when the player presses jump button.
    /// </summary>
    private void Jump()
    {
        //The player can't jump if it isn't grounded
        if (!isGrounded)
            return;
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce, rigidBody.velocity.y); //Start jumping by setting the player's y velocity to the jump force
    }

    /// <summary>
    /// Checks if the player is grounded
    /// </summary>
    /// <returns></returns>
    private bool IsGrounded()
    {
        //Raycast down from the player to check if there is ground beneath the player
        return Physics.Raycast(groundCheckPoint.transform.position, Vector3.down, groundedDistance);
    }

    private void Update()
    {
        //Set the animator's walking status and send walking sounds to zombies
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
        HandleCameraRotation(); //Handle the camera's rotation
    }

    private void FixedUpdate()
    {
        isGrounded = IsGrounded(); //Check if the player is grounded
        HandleMovement(); //Handle the player's movement
    }

    /// <summary>
    /// Handles the player's movement. Should be called in FixedUpdate.
    /// </summary>
    private void HandleMovement()
    {
        float speedToUse = ((shiftPressed/* && stamina_check!*/) ? runningSpeed : walkingSpeed); //The maximum speed to use in the target velocity
        Vector3 xVelocity = transform.right * movement.x * speedToUse; //The player's target velocity in the x axis relative to player's rotation from the input's (Vector2) x axis (a and d buttons)
        Vector3 zVelocity = transform.forward * movement.y * speedToUse; //The player's target velocity in the z axis relative to player's rotation from the input's (Vector2) y axis (w and s buttons)
        float speedChangeToUse = isGrounded ? speedChange : jumpSpeedChange; //The lerping speed to use in the velocity changing. The value is supposed to be lower when the player is jumping so the player can control the velocity less.

        Vector3 newVelocity = Vector3.Lerp(previousVelocity, xVelocity + zVelocity, speedChangeToUse); //Lerp smoothly from previous velocity to the target velocity using the calculated speed change. The target velocity is sum of the target velocitys in the player's relative x and z axis.
        newVelocity[1] = rigidBody.velocity.y; //Set the new velocity's y to the y of the current velocity so that the y of the new velocity won't be always 0
        rigidBody.velocity = newVelocity; //Set the new velocity to the rigidbody's velocity

        previousVelocity = newVelocity; //Set the previous velocity variable used in the next running of this method
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
