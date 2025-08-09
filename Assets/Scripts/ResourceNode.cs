using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public int resourceAmount = 100;

    public void Collect(int amount)
    {
        resourceAmount -= amount;
        if (resourceAmount <= 0)
        {
            Destroy(gameObject);
        }
    }
}