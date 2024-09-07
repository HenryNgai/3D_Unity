using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement variables
    public float speed  = 3f; 
    public float runSpeed = 6f;
    public float walkSpeed = 3f;
    public float jumpForce = 5f;
    public bool isRunning = false;

    // Attack variables
    private bool isAttacking = false;

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
    private AnimatorStateInfo stateInfo;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        animator = GetComponent<Animator>();

        // Lock the cursor in the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {  
        // Handle mouse input for rotating the camera and player body
        HandleMouseLook();

        // Handles attacks
        HandleAttack();

        // Handles Movement
        HandleMovement();

        // Handle animation updates
        HandleAnimations();
    }

    void HandleMovement()
    {
        // Get latest animation state
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("attack"))
        {
            Debug.Log("Skipping movement since we're still in attack animation");
        }
        else
        {
            Debug.Log("Not attacking. Able to move character");
            // Check if the player is grounded
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            // Get input for movement
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            
            // Calculating Running
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                isRunning = true;
            }
            else{
                isRunning = false;
            }
            speed = isRunning ? runSpeed : walkSpeed;

            // Move the player using MovePosition instead of directly modifying velocity
            Vector3 move = transform.right * moveX + transform.forward * moveZ;
            Vector3 targetPosition = rb.position + move * (isGrounded ? speed : speed * 0.5f) * Time.deltaTime;  // Move slower if not grounded
            rb.MovePosition(targetPosition);
        }


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
        if (moveZ > 0) // Moving Forward
        {
            animator.SetBool("isWalkingForward", true);
            animator.SetBool("isWalkingBackward", false);
        }
        else if (moveZ < 0) // Moving Backwards
        {
            animator.SetBool("isWalkingForward", false);
            animator.SetBool("isWalkingBackward", true);
        }
        else // Idle
        {
            animator.SetBool("isWalkingForward", false);
            animator.SetBool("isWalkingBackward", false);
        }

        // Check Running
        animator.SetBool("isRunning", isRunning);
    }


    void HandleAttack()
    {   
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            Attack();
        }
        HandleAttackCompletion();

    }
    void Attack(){
        animator.SetTrigger("attack");
        isAttacking = true;
        Debug.Log("Attack triggered");
    }

    void HandleAttackCompletion(){
        // Check if attack anim is still playing
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Annimation is no longer attacking
        if (isAttacking && !stateInfo.IsName("attack"))
        {   //Reset to allow for attack again
            isAttacking = false;
            Debug.Log("Able to attack again");
        }


    }
}
