using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 0.5f;
    public float minX = -50f, maxX = 50f, minZ = -50f, maxZ = 50f;

    public float zoomSpeed = 10f;
    public float minY = 10f, maxY = 60f; // Camera height limits

    private Vector2 lastPanPosition;
    private int panFingerId; // Touch finger ID

    void Update()
    {
        BuildingPlacer placer = FindFirstObjectByType<BuildingPlacer>();
        if (placer != null && placer.IsBuildingSelected)
            return;

        // Touch pan for mobile
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastPanPosition = touch.position;
                panFingerId = touch.fingerId;
            }
            else if (touch.phase == TouchPhase.Moved && touch.fingerId == panFingerId)
            {
                PanCamera(touch.position);
                lastPanPosition = touch.position;
            }
        }

        // Touch pinch zoom for mobile
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            float prevDist = (touch0.position - touch0.deltaPosition - (touch1.position - touch1.deltaPosition)).magnitude;
            float currDist = (touch0.position - touch1.position).magnitude;
            float delta = currDist - prevDist;

            ZoomCamera(-delta * zoomSpeed * Time.deltaTime * 0.1f);
        }

        // Mouse drag pan for Editor
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            lastPanPosition = Input.mousePosition;
        if (Input.GetMouseButton(0))
        {
            PanCamera((Vector2)Input.mousePosition);
            lastPanPosition = Input.mousePosition;
        }

        // Mouse wheel zoom for Editor
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            ZoomCamera(-scroll * zoomSpeed * 100f * Time.deltaTime);
        }
        #endif
    }

    void PanCamera(Vector2 newPanPosition)
    {
        Vector2 delta = newPanPosition - lastPanPosition;
        Vector3 move = new Vector3(-delta.x * panSpeed * Time.deltaTime, 0, -delta.y * panSpeed * Time.deltaTime);

        Vector3 newPos = transform.position + move;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);

        transform.position = newPos;
    }

    void ZoomCamera(float delta)
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y + delta, minY, maxY);
        transform.position = pos;
    }
}