using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public GameObject buildingPrefab; // Assign in Inspector
    public GameObject progressBarPrefab; // Assign in Inspector
    public float placementMargin = 2.5f; // 0.5f * 5
    public int goldCost = 50;
    public int woodCost = 50;
    public float constructionTime = 10f; // seconds

    private Dictionary<GameObject, bool> constructionCancelled = new Dictionary<GameObject, bool>();

    void Update()
    {
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() &&
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null)
        {
            return; // Ignore clicks over UI controls
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Raycast from mouse to ground
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    // Get the size of the building's collider
                    Collider buildingCollider = buildingPrefab.GetComponent<Collider>();
                    Vector3 size = buildingCollider.bounds.size;

                    // Add margin to the size for overlap check
                    Vector3 margin = new Vector3(placementMargin, placementMargin, placementMargin);
                    Vector3 checkSize = (size / 2f) + margin;

                    // Calculate the correct center for overlap check
                    float yOffset = size.y / 2f;
                    Vector3 center = hit.point + Vector3.up * yOffset;

                    // Check for overlap with other buildings
                    Collider[] overlaps = Physics.OverlapBox(center, checkSize, Quaternion.identity, LayerMask.GetMask("Building"));
                    if (overlaps.Length == 0)
                    {
                        // Check resource cost
                        if (ResourceManager.Instance.gold >= goldCost && ResourceManager.Instance.wood >= woodCost)
                        {
                            WorkerDrag worker = FindNearestIdleWorker(hit.point);
                            if (worker == null)
                            {
                                Debug.Log("No available worker to build!");
                                return;
                            }
                            worker.isBusy = true;

                            // Get the building's collider size
                            float buildingRadius = Mathf.Max(size.x, size.z) / 2f;
                            float workerMargin = 0.2f; // Adjust as needed

                            // Calculate a point beside the building (to the right of the building)
                            Vector3 direction = (worker.transform.position - hit.point).normalized;
                            if (direction == Vector3.zero) direction = Vector3.right; // fallback if worker is at the same spot
                            Vector3 besidePoint = hit.point + direction * (buildingRadius + workerMargin);

                            worker.GoTo(besidePoint);

                            // Deduct resources
                            ResourceManager.Instance.gold -= goldCost;
                            ResourceManager.Instance.wood -= woodCost;

                            // Instantiate at hit.point first
                            GameObject building = Instantiate(buildingPrefab, hit.point, Quaternion.identity);

                            // Get the collider from the instantiated building
                            Collider col = building.GetComponent<Collider>();
                            if (col != null)
                            {
                                // Move the building up so its base sits on the ground
                                float buildingYOffset = col.bounds.extents.y;
                                building.transform.position = new Vector3(hit.point.x, hit.point.y + buildingYOffset, hit.point.z);

                                // Calculate a point beside the building (after building is placed)
                                float buildingRadiusFinal = Mathf.Max(col.bounds.extents.x, col.bounds.extents.z);
                                float workerMarginFinal = 1.0f; // Increase margin for larger buildings
                                Vector3 directionFinal = (worker.transform.position - building.transform.position).normalized;
                                if (directionFinal == Vector3.zero) directionFinal = Vector3.right;
                                Vector3 besidePointFinal = building.transform.position + directionFinal * (buildingRadiusFinal + workerMarginFinal);

                                worker.GoTo(besidePointFinal);

                                // Start construction coroutine, pass besidePoint
                                Coroutine coroutine = StartCoroutine(ConstructBuilding(building, worker, besidePointFinal));
                                worker.StartConstruction(building, coroutine, goldCost, woodCost);
                            }
                        }
                        else
                        {
                            Debug.Log("Not enough resources to build!");
                        }
                    }
                    else
                    {
                        Debug.Log("Cannot place building here: space is occupied or too close to another building.");
                    }
                }
            }
        }
    }

    private System.Collections.IEnumerator ConstructBuilding(GameObject building, WorkerDrag worker, Vector3 besidePoint)
    {
        Renderer rend = building.GetComponentInChildren<Renderer>();
        if (rend != null)
            rend.material.color = Color.yellow;

        GameObject barObj = Instantiate(progressBarPrefab);
        Collider col = building.GetComponent<Collider>();
        float barHeight = col != null ? col.bounds.extents.y * 1.1f : 10f;
        barObj.transform.localPosition = new Vector3(building.transform.position.x, barHeight, building.transform.position.z);
        var slider = barObj.GetComponentInChildren<UnityEngine.UI.Slider>();

        // Register cancellation flag
        constructionCancelled[building] = false;
        worker.SetConstructionProgressBar(barObj);

        // Wait for worker to arrive at besidePoint
        while (Vector3.Distance(worker.transform.position, besidePoint) > 3f)
        {
            if (constructionCancelled[building]) yield break;
            yield return null;
        }

        float elapsed = 0f;
        while (elapsed < constructionTime)
        {
            if (constructionCancelled[building])
            {
                // Clean up if cancelled
                if (barObj != null) Destroy(barObj);
                yield break;
            }
            elapsed += Time.deltaTime;
            if (slider != null)
                slider.value = Mathf.Clamp01(elapsed / constructionTime);
            yield return null;
        }

        // After construction is complete:
        if (rend != null)
            rend.material.color = Color.white;

        Turret turret = building.GetComponent<Turret>();
        if (turret != null)
            turret.isConstructed = true;

        Destroy(barObj);

        // Reset worker status
        worker.isBusy = false;
        worker.state = WorkerDrag.WorkerState.Idle;
        worker.currentBuilding = null;
        worker.constructionCoroutine = null;
        worker.lastGoldCost = 0;
        worker.lastWoodCost = 0;
        worker.constructionProgressBar = null;

        constructionCancelled.Remove(building);
        yield break;
    }

    private WorkerDrag FindNearestIdleWorker(Vector3 position)
    {
        WorkerDrag[] workers = FindObjectsByType<WorkerDrag>(FindObjectsSortMode.None);
        WorkerDrag nearest = null;
        float minDist = float.MaxValue;
        foreach (var worker in workers)
        {
            if (!worker.isBusy)
            {
                float dist = Vector3.Distance(worker.transform.position, position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = worker;
                }
            }
        }
        return nearest;
    }
}