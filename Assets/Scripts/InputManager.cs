using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    // References to other systems
    private CameraController cameraController;
    private UnitController unitController;
    private BuildingPlacer buildingPlacer;

    // Input Actions (assigned in the Inspector)
    public InputAction touchAction; // Press action for touch
    public InputAction zoomAction;  // Delta action for zooming

    private bool isTouchingUI = false; // Flag to track if the touch is over a UI element
    private bool isTouchStarted = false; // Flag to track if a touch started

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void OnEnable()
    {
        // Enable input actions
        touchAction.Enable();
        zoomAction.Enable();

        // Bind input actions
        touchAction.started += OnTouchStarted;
        touchAction.canceled += OnTouchEnded;
    }

    void OnDisable()
    {
        // Disable input actions
        touchAction.Disable();
        zoomAction.Disable();

        // Unbind input actions
        touchAction.started -= OnTouchStarted;
        touchAction.canceled -= OnTouchEnded;
    }

    void Start()
    {
        // Find references to other systems
        cameraController = FindFirstObjectByType<CameraController>();
        if (cameraController == null)
            Debug.LogWarning("CameraController not found!");

        unitController = FindFirstObjectByType<UnitController>();
        if (unitController == null)
            Debug.LogWarning("UnitController not found!");

        buildingPlacer = FindFirstObjectByType<BuildingPlacer>();
        if (buildingPlacer == null)
            Debug.LogWarning("BuildingPlacer not found!");
    }

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        isTouchStarted = true; // Mark that a touch has started

        if (!IsInRestrictedMode())
        {
            Vector2 initialPanPosition = GetPointerPosition();
            Debug.Log($"Starting camera panning at position: {initialPanPosition}");
            cameraController?.StartPanning(initialPanPosition); // Pass the initial pan position
        }
    }

    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        if (isTouchingUI)
        {
            Debug.Log("Touch ended on a UI element. Ignoring.");
            isTouchingUI = false;
            isTouchStarted = false; // Reset the touch started flag
            return;
        }

        // Handle building placement
        if (buildingPlacer != null && buildingPlacer.IsBuildingSelected)
        {
            Vector2 screenPosition = GetPointerPosition();
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    buildingPlacer.PlaceBuilding(hit);
                }
            }
        }
        else if (unitController != null && unitController.isGatherMode)
        {
            // Handle unit gathering
            Vector2 screenPosition = GetPointerPosition();
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                unitController.GatherUnits(hit.point);
            }
        }

        if (!IsInRestrictedMode())
        {
            Debug.Log("Stopping camera panning.");
            cameraController?.StopPanning();
        }

        isTouchStarted = false; // Reset the touch started flag
    }

    void Update()
    {
        // Perform the UI check in Update
        if (isTouchStarted)
        {
            if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
            // Try to get the UI element under the pointer
            var pointerEventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current)
            {
                position = GetPointerPosition()
            };

            var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerEventData, results);

            if (results.Count > 0)
            {
                var uiElement = results[0].gameObject;
                Debug.Log($"Touch started on UI element: {uiElement.name}");
            }
            else
            {
                Debug.Log("Touch started on a UI element, but could not identify which.");
            }

            isTouchingUI = true;
            }
            else
            {
                isTouchingUI = false;

                if (!IsInRestrictedMode())
                {
                    Debug.Log("Starting camera panning.");
                    cameraController?.StartPanning(GetPointerPosition());
                }
            }

            isTouchStarted = false; // Reset the touch started flag after processing
        }

        // Handle camera panning
        if (cameraController != null && cameraController.IsPanning && !IsInRestrictedMode())
        {
            Vector2 currentPanPosition = GetPointerPosition();
            Debug.Log($"Panning camera to position: {currentPanPosition}");
            cameraController.PanCamera(currentPanPosition);
        }

        // Handle zooming
        if (zoomAction != null && !IsInRestrictedMode())
        {
            Vector2 zoomDelta = zoomAction.ReadValue<Vector2>();
            if (zoomDelta != Vector2.zero)
            {
                Debug.Log($"Zooming camera with delta: {zoomDelta.y}");
                cameraController?.ZoomCamera(zoomDelta.y);
            }
        }
    }

    // Helper method to check if the player is in a restricted mode
    private bool IsInRestrictedMode()
    {
        // Restrict camera movement if in construction mode or gather mode
        if ((buildingPlacer != null && buildingPlacer.IsBuildingSelected) ||
            (unitController != null && unitController.isGatherMode))
        {
            return true;
        }
        return false;
    }

    // Helper method to get pointer position
    private Vector2 GetPointerPosition()
    {
        if (Touchscreen.current != null)
        {
            return Touchscreen.current.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }
        return Vector2.zero;
    }
}