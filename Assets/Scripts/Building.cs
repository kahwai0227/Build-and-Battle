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

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            DestroyBuilding();
        }
    }

    protected virtual void DestroyBuilding()
    {
        Debug.Log($"{gameObject.name} has been destroyed!");
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (OnDestroyed != null)
            OnDestroyed.Invoke();
    }
}