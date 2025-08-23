public class TownhallHealth : Building
{
    public float maxHealth = 100f;
    void Start()
    {
        health = maxHealth; // Initialize health from the Building class
    }

    public override void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            GameManager.Instance.GameOver(false);
            gameObject.SetActive(false); // Hide the townhall, not the UI
        }
    }
}