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
            healthText.text = $"Townhall Health: {townhall.currentHealth}/{townhall.maxHealth}";

            if (townhall.currentHealth <= 0)
            {
                Debug.Log("Game Over! Townhall destroyed.");
                GameManager.Instance.GameOver(false);
            }
        }
    }
}