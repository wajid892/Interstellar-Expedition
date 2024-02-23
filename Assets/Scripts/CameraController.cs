using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 3f;
    public float zoomSpeed = 5f;

    public bool invertXAxis = false;
    public bool invertYAxis = false;

    void Update()
    {
        // Movement controls
        float horizontalInput = invertXAxis ? -Input.GetAxis("Horizontal") : Input.GetAxis("Horizontal");
        float verticalInput = invertYAxis ? -Input.GetAxis("Vertical") : Input.GetAxis("Vertical");
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * movementSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // Rotation controls
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            mouseX *= invertXAxis ? -1 : 1;
            mouseY *= invertYAxis ? -1 : 1;

            Vector3 rotation = new Vector3(-mouseY, mouseX, 0f) * rotationSpeed * Time.deltaTime;
            transform.Rotate(rotation, Space.World);
        }

        // Zoom controls
        Vector3 zoom = new Vector3(0f, 0f, zoomInput) * zoomSpeed * Time.deltaTime;
        transform.Translate(zoom, Space.Self);
    }
}
