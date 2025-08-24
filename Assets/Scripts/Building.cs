using System.Runtime.CompilerServices;
using UnityEngine;

public class Building : MonoBehaviour
{
    public event System.Action OnDestroyed;
    public int goldCost = 50;
    public int woodCost = 50;
    public float constructionTime = 10f;

    public bool isConstructed = false;

    public float health = 100f; // Default health for buildings

    // Reference to the construction progress bar
    public GameObject constructionProgressBar;

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {health}");

        if (health <= 0)
        {
            DestroyBuilding();
        }
    }

    protected virtual void DestroyBuilding()
    {
        Debug.Log($"{gameObject.name} has been destroyed!");

        // Destroy the construction progress bar if it exists
        if (constructionProgressBar != null)
        {
            Destroy(constructionProgressBar);
            Debug.Log("Construction progress bar destroyed.");
        }

        // Additional cleanup logic (if needed)
        OnDestroyed?.Invoke();

        // Destroy the building itself
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (OnDestroyed != null)
            OnDestroyed.Invoke();
    }
}