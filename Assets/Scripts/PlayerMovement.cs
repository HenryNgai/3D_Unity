using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;           // Walking speed
    public float mouseSensitivity = 200f; // Sensitivity for mouse movement
    public float jumpForce = 5f;       // Jumping force

    public Transform playerBody;       // Reference to the player body (root object)
    public Animator animator;          // Reference to player animator
    //public Camera playerCamera;        // Reference to player camera

    public static event Action OnFootstep;  // Event to broadcast footsteps
    public static event Action OnStopFootstep; // Event to broadcast stopping

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

    private bool isIdle;
    private bool isWalking;
    private bool isWalkingBackwards;
    
    private bool isSlashing;           // Flag for slashing state
    private bool canSlash = true;      // Flag to ensure slashing only happens once per press

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Lock the cursor in the game window
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Handle input and mouse look in Update
        HandleInput();
        HandleMouseLook();
        UpdateAnimator();
        HandleSound();
    }

    void FixedUpdate()
    {
        // Handle physics-based movement and jumping in FixedUpdate
        HandleMovement();
    }

    void HandleSound()
    {
        if ((isWalking || isWalkingBackwards) && !isIdle)
        {
            // Broadcast the footstep event to any subscribers
            OnFootstep?.Invoke();
        }
        else if (isIdle)
        {
            OnStopFootstep?.Invoke();
        }
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

        // Slashing input - trigger slashing only once when pressing space
        if (Input.GetMouseButtonDown(0) && canSlash)
        {
            isSlashing = true;
            canSlash = false; // Disable continuous slashing while the key is held
            Debug.Log("Pressing Left Click - canSlash:" + canSlash);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            // Re-enable slashing when space is released
            canSlash = true;
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
        //playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
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

    void UpdateAnimator()
    {
        // Check if player is walking or idle based on input
        if (moveX != 0 || moveZ != 0)
        {
            if (moveZ < 0)
            {
                isWalkingBackwards = true;
                isWalking = false;
                isIdle = false;
            }
            else
            {
                isWalkingBackwards = false;
                isWalking = true;
                isIdle = false;
            }
        }
        else
        {
            isWalking = false;
            isIdle = true;
            isWalkingBackwards = false;
        }

        // Set the flags in the Animator
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isWalkingBackwards", isWalkingBackwards); // Add this to trigger backward walking animation

        // Handle slashing animation
        if (isSlashing)
        {
            animator.SetTrigger("slash");  // Set trigger for slashing animation
            isSlashing = false;            // Reset slashing flag after triggering animation
        }
    }
}
