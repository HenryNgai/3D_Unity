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
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        // Calculate movement direction
        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

        // Apply movement (only on the XZ plane)
        Vector3 moveVelocity = moveDirection.normalized * currentSpeed;
        Vector3 velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);

        rb.velocity = velocity;
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
        // Get movement input for forward/backward direction
        float moveZ = Input.GetAxisRaw("Vertical");

        // Set the 'speed' parameter to control the forward/backward animations
        float speedValue = Mathf.Abs(moveZ); // Absolute value since speed can be negative for backward
        animator.SetFloat("speed", speedValue);

        // Set 'isRunning' based on whether Left Shift is pressed
        animator.SetBool("isRunning", Input.GetKey(KeyCode.LeftShift));

        // Handle forward or backward walking/running
        if (moveZ > 0)
        {
            animator.SetBool("isWalkingForward", true);
            animator.SetBool("isWalkingBackward", false);
        }
        else if (moveZ < 0)
        {
            animator.SetBool("isWalkingForward", false);
            animator.SetBool("isWalkingBackward", true);
        }
        else
        {
            animator.SetBool("isWalkingForward", false);
            animator.SetBool("isWalkingBackward", false);
        }

        // Handle jump animation
        Debug.Log("isGrounded: " + isGrounded);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetBool("isJumping", true);
        }
                // Check if the player has landed (grounded again)
        else if (!isGrounded && animator.GetBool("isJumping"))
        {
            // Reset the jump animation when player lands
            animator.SetBool("isJumping", false);
        }
    }
}
