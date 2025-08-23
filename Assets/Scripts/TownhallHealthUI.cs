using UnityEngine;
using TMPro;

public class TownhallHealthUI : MonoBehaviour
{
    public TownhallHealth townhall;
    public TMP_Text healthText;

    void Update()
    {
        if (townhall != null && healthText != null)
        {
            healthText.text = $"Townhall Health: {townhall.health}/{townhall.maxHealth}";
        }
    }
}