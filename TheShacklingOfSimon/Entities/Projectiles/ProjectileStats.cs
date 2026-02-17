namespace TheShacklingOfSimon.Entities.Projectiles;

public record struct ProjectileStats
{
    public float Damage;
    public float Speed;

    public ProjectileStats(float damage, float speed)
    {
        Damage = damage;
        Speed = speed;
    }
}