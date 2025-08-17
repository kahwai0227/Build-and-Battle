// Example for a turret button
using UnityEngine;
using UnityEngine.UI;

public class BuildTurretButton : MonoBehaviour
{
    public BuildingPlacer placer;
    public Image buttonImage;
    public Color selectedColor = Color.red;
    public Color normalColor = Color.white;
    public GameObject buildingPrefab;

    public void OnButtonClick()
    {
        if (placer == null || buildingPrefab == null)
        {
            Debug.LogError("BuildingPlacer or buildingPrefab is not set!");
            return;
        }
        // Toggle building selection
        Debug.Log($"Button clicked for building: {buildingPrefab.name}");
        placer.ToggleBuildingSelection(buildingPrefab);
        UpdateButtonColor();
    }

    void Update()
    {
        UpdateButtonColor();
    }

    void UpdateButtonColor()
    {
        if (placer.buildingPrefab == buildingPrefab)
            buttonImage.color = selectedColor;
        else
            buttonImage.color = normalColor;
    }
}