namespace TheShacklingOfSimon.Entities.Projectiles;

public record struct ProjectileStats
{
    public int Damage;
    public float Speed;

    public ProjectileStats(int damage, float speed)
    {
        Damage = damage;
        Speed = speed;
    }
}