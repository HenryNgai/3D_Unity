using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement variables
    public float speed = 3f; 
    public float jumpForce = 5f;
    public bool isRunning = false;

    // Ground check variables
    public bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    // Camera and mouse look variables
    public float mouseSensitivity = 200f;
    public Transform playerBody;       // Reference to player's body for rotating
    private float xRotation = 0f;

    // Physics Variables
    private Rigidbody rb;
    
    // Animation Variables
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Lock the cursor in the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Handle mouse input for rotating the camera and player body
        HandleMouseLook();

        // Handle player movement
        HandleMovement();

        // Handle animation updates
        HandleAnimations();
    }

    void HandleMovement()
    {
        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Get input for movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        
        // Calculating Running
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
            isRunning = true;
            speed = 6f;
        }
        else{
            isRunning = false;
            speed = 3f;
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        
        // Move the player by modifying the velocity of the Rigidbody
        Vector3 velocity = move * speed;
        velocity.y = rb.velocity.y; // Preserve vertical velocity
        rb.velocity = velocity;

    }

    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the head up and down (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 45f);  // Clamp rotation to avoid unnatural angles

        // Rotate the player body left and right (yaw)
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void HandleAnimations()
    {
        // Get input for movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Animate z (forward and backwards) correctly
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

        // Check Running
        animator.SetBool("isRunning", isRunning);
    }
}
