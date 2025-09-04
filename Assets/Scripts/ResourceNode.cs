using UnityEngine;
using UnityEngine.InputSystem;

public enum ResourceType { Wood, Gold }

public class ResourceNode : MonoBehaviour
{
    public ResourceType resourceType = ResourceType.Wood;
    public int resourceAmount = 100;
    public int collectPerTrip = 10;

    private InputAction touchAction;

    void Awake()
    {
        // Initialize the InputAction for detecting touch input
        touchAction = new InputAction(type: InputActionType.PassThrough);
        touchAction.AddBinding("<Touchscreen>/press"); // Touch press
    }

    void OnEnable()
    {
        touchAction.Enable();
        touchAction.performed += OnTouchPerformed;
    }

    void OnDisable()
    {
        touchAction.Disable();
        touchAction.performed -= OnTouchPerformed;
    }

    public bool Collect()
    {
        if (resourceAmount >= collectPerTrip)
        {
            resourceAmount -= collectPerTrip;
            return true;
        }
        else if (resourceAmount > 0)
        {
            resourceAmount = 0;
            return true;
        }
        return false;
    }

    public bool IsDepleted()
    {
        return resourceAmount <= 0;
    }

    private void OnTouchPerformed(InputAction.CallbackContext context)
    {
        // Perform a raycast to check if the touch is on this resource node
        Vector2 screenPosition = Pointer.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == gameObject) // Check if the hit object is this resource node
            {
                WorkerDrag[] workers = FindObjectsByType<WorkerDrag>(FindObjectsSortMode.None);
                foreach (var worker in workers)
                {
                    if (!worker.isBusy)
                    {
                        worker.AssignResource(this);
                        break;
                    }
                }
            }
        }
    }
}