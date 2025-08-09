using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    public TMP_Text resourceTMPText; // Assign in Inspector

    void Update()
    {
        if (ResourceManager.Instance != null && resourceTMPText != null)
        {
            resourceTMPText.text = $"Gold: {ResourceManager.Instance.gold}\nWood: {ResourceManager.Instance.wood}";
        }
    }
}