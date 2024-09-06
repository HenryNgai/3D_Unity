using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;           // Walking speed
    public float mouseSensitivity = 200f; // Sensitivity for mouse movement
    public float jumpForce = 5f;       // Jumping force

    public Transform playerBody;       // Reference to the player body (root object)
    // public Transform playerCamera;     // Reference to the camera (child object)
    public LayerMask groundMask;       // Layer mask for ground detection
    public float groundDistance = 0.4f; // Distance to check for the ground

    private Rigidbody rb;
    private float xRotation = 0f;      // Rotation of the camera on the X-axis
    private bool isGrounded;           // Ground check
    public Transform groundCheck;      // Empty gameObject under player feet for checking ground

    // Store input values for movement
    private float moveX;
    private float moveZ;
    private bool jumpInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Lock the cursor in the game window
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Handle input and mouse look in Update
        HandleInput();
        HandleMouseLook();
    }

    void FixedUpdate()
    {
        // Handle physics-based movement and jumping in FixedUpdate
        HandleMovement();
    }

    void HandleInput()
    {
        // Get movement input (WASD)
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        // Jumping input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpInput = true;
        }
        else
        {
            jumpInput = false;
        }
    }

    void HandleMouseLook()
    {
        // Get mouse input for look direction
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the player horizontally (Y-axis)
        playerBody.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically (X-axis)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevent camera from flipping
        //playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        // Check if grounded using a sphere overlap (ground detection)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Apply movement based on input collected in Update
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        rb.velocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);

        // Apply jump force if jumpInput is true
        if (jumpInput && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }


}
