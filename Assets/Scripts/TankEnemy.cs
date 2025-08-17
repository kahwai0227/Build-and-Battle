public class TankEnemy : Enemy
{
    public override void Start()
    {
        speed = 5f; // Faster speed
        health = 30; // Less health
        base.Start();
    }
}