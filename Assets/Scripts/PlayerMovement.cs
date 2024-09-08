using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RPGPlayerMovement : MonoBehaviour
{
    // Movement settings
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpForce = 7f;

    // Ground check settings
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    // Camera and player control settings
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;  // Assign the camera's transform to this in the Inspector

    private Rigidbody rb;
    private bool isGrounded;
    private float currentSpeed;
    private float xRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
    }

    void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Get movement input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

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
}
