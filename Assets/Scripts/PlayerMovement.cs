using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement

    private bool isFacingRight = true; // Track which direction the player is facing

    void Update()
    {
        // Get input for movement along the X and Z axes
        float moveX = Input.GetAxis("Horizontal"); // Left/Right or A/D for X-axis movement
        float moveZ = Input.GetAxis("Vertical");   // Forward/Back or W/S for Z-axis movement

        // Create a movement vector (moving only along the X and Z axis)
        Vector3 move = new Vector3(moveX, 0, moveZ);

        // Move the player by the movement vector times the speed and time delta
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        // Flip the player when moving left or right
        if (moveX > 0 && !isFacingRight)
        {
            // Moving right and currently facing left, so flip
            Flip();
        }
        else if (moveX < 0 && isFacingRight)
        {
            // Moving left and currently facing right, so flip
            Flip();
        }
    }

    // Flip the player by scaling the X-axis
    void Flip()
    {
        isFacingRight = !isFacingRight; // Toggle the direction the player is facing

        // Flip the player's local scale on the X-axis
        Vector3 playerScale = transform.localScale;
        playerScale.x *= -1;
        transform.localScale = playerScale;
    }
}
