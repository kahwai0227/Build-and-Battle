using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    public int gold = 0;
    public int wood = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddGold(int amount) => gold += amount;
    public void AddWood(int amount) => wood += amount;
    public bool SpendGold(int amount) { if (gold >= amount) { gold -= amount; return true; } return false; }
    public bool SpendWood(int amount) { if (wood >= amount) { wood -= amount; return true; } return false; }
}