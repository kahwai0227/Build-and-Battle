using UnityEngine;

public class TownhallHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOver(false);
            gameObject.SetActive(false); // Hide the townhall, not the UI
        }
    }
}