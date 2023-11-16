using UnityEngine;

public class CameraPeek : MonoBehaviour
{
    public float peekDistance = 5f;
    public float peekSpeed = 5f;

    private bool isPeeking = false;

    void Update()
    {
        // Check for R3 button press
        if (Input.GetAxis("CameraLookHorizontal") > 0 || Input.GetAxis("CameraLookVertical") > 0) // Check the correct button index for R3
        {
            Debug.Log("Joy stick is down");
            isPeeking = true;
        }
        // Check for R3 button release
        else  // Check the correct button index for R3
        {
            isPeeking = false;
        }

        // Peek logic
        if (isPeeking)
        {
            // Get input from right stick
            float horizontalInput = Input.GetAxis("CameraLookHorizontal");
            float verticalInput = Input.GetAxis("CameraLookVertical");

            // Calculate the peek direction
            Vector3 peekDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;

            // Move the camera position towards the peek direction
            transform.position += peekDirection * peekSpeed * Time.deltaTime;

            // Clamp the camera position to avoid going too far
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -peekDistance, peekDistance),
                Mathf.Clamp(transform.position.y, -peekDistance, peekDistance),
                transform.position.z
            );
        }
    }
}
