using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Projectiles;

namespace TheShacklingOfSimon.Weapons;

public interface IWeapon
{
    string Name { get; }
    string Description { get; }
    IProjectile Projectile { get; }

    /*
     * pos is the starting position of the Projectile
     * direction is the direction the Projectile should go
     * stats is the various stats of the Projectile, including:
     *      The damage of the Projectile,
     *      The speed of the Projectile
     */
    void Fire(Vector2 pos, Vector2 direction, ProjectileStats stats);
}