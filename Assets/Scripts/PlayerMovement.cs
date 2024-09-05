using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement

    void Update()
    {
        // Get the input for the X and Z axes
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow for X-axis
        float moveZ = Input.GetAxis("Vertical"); // W/S or Up/Down Arrow for Z-axis

        // Create a movement vector
        Vector3 move = new Vector3(moveX, 0, moveZ);

        // Move the player
        transform.position += move * moveSpeed * Time.deltaTime;
    }
}
