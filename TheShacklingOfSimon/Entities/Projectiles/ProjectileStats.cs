namespace TheShacklingOfSimon.Entities.Projectiles;

public record struct ProjectileStats
{
    public int Damage;
    public float Speed;
    public ProjectileOwner OwnerType;

    public ProjectileStats(int damage, float speed, ProjectileOwner ownerType)
    {
        Damage = damage;
        Speed = speed;
        OwnerType = ownerType;
    }
}