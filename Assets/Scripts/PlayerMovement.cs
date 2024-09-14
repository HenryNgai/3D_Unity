using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RPGPlayerMovement : MonoBehaviour
{
    // Movement settings
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpForce = 7f;
    private float currentSpeed;

    // Ground check settings
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    // Camera and player control settings
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;  // Assign the camera's transform to this in the Inspector

    // Rigid body settings
    private Rigidbody rb;
    private float xRotation = 0f;

    //Animator settings
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true; // Prevent the Rigidbody from rotating

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Handle camera and character rotation
        HandleMouseLook();

        // Handle movement input
        HandleMovement();

        // Handle jumping input
        HandleJump();

        //Update Animations
        UpdateAnimations();
    }

    void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Get movement input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Determine if the player is running or walking
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        
        // Calculate target speed
        float targetSpeed = isRunning ? runSpeed : walkSpeed;

        // Normalize input to prevent faster diagonal movement
        Vector3 inputDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // Calculate movement direction relative to the player's orientation
        Vector3 moveDirection = transform.TransformDirection(inputDirection);

        // Apply movement (only on the XZ plane)
        Vector3 moveVelocity = moveDirection * targetSpeed;

         // Update the Rigidbody's velocity, preserving Y-axis movement (gravity)
        Vector3 velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
        rb.velocity = velocity;

        // Calculate currentSpeed for animation purposes
        // This represents the speed in the forward/backward direction
        currentSpeed = Vector3.Dot(moveDirection, transform.forward) * targetSpeed;
    }

    void HandleJump()
    {
        // Jump when space is pressed and the player is grounded
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the camera up and down (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Clamp rotation to avoid unnatural angles
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the player body left and right (yaw)
        transform.Rotate(Vector3.up * mouseX);
    }

    void UpdateAnimations()
    {
        animator.SetFloat("speed", currentSpeed);
        // Handle jump animation
        // Debug.Log("isGrounded: " + isGrounded);
        // if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        // {
        //     animator.SetBool("isJumping", true);
        // }
        //         // Check if the player has landed (grounded again)
        // else if (!isGrounded && animator.GetBool("isJumping"))
        // {
        //     // Reset the jump animation when player lands
        //     animator.SetBool("isJumping", false);
        // }
    }
}
