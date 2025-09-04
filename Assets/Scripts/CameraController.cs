using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 0.5f;
    public float zoomSpeed = 10f;
    public float minY = 10f, maxY = 60f; // Camera height limits
    public float minX = -50f, maxX = 50f, minZ = -50f, maxZ = 50f;

    private Vector2 lastPanPosition;
    public bool IsPanning { get; private set; } = false;

    public void StartPanning(Vector2 initialPanPosition)
    {
        IsPanning = true;
        lastPanPosition = initialPanPosition; // Initialize the last pan position
    }

    public void StopPanning()
    {
        IsPanning = false;
    }

    public void PanCamera(Vector2 newPanPosition)
    {
        if (!IsPanning) return;

        // Calculate the delta between the current and last pan positions
        Vector2 delta = newPanPosition - lastPanPosition;

        // Apply the delta to move the camera
        Vector3 move = new Vector3(-delta.x * panSpeed, 0, -delta.y * panSpeed);

        // Clamp the new position within the defined boundaries
        Vector3 newPos = transform.position + move;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);
        newPos.y = transform.position.y; // Keep the Y position unchanged

        transform.position = newPos;

        // Update the last pan position
        lastPanPosition = newPanPosition;
    }

    public void ZoomCamera(float delta)
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y - delta * zoomSpeed * Time.deltaTime, minY, maxY);
        transform.position = pos;
    }
}