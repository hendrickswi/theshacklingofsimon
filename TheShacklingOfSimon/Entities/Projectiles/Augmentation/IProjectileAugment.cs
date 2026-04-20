namespace TheShacklingOfSimon.Entities.Projectiles.Augmentation;

public interface IProjectileAugment
{
    IProjectile ApplyTo(IProjectile projectile);
}