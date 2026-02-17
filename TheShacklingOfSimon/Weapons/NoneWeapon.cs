using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;

namespace TheShacklingOfSimon.Weapons;

public class NoneWeapon : IWeapon
{
    public string Name { get; set; }
    public string Description { get; set; }

    public void Fire(Vector2 position, Vector2 direction, ProjectileStats stats)
    {
        // No-op
    }
}