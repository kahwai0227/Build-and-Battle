using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public GameObject buildingPrefab; // Assign in Inspector

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast from mouse to ground
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    Instantiate(buildingPrefab, hit.point, Quaternion.identity);
                }
            }
        }
    }
}