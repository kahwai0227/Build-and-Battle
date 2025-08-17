public class FastEnemy : Enemy
{
    public override void Start()
    {
        speed = 20f; // Faster speed
        health = 5; // Less health
        base.Start();
    }
}