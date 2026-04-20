#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;

#endregion

namespace TheShacklingOfSimon.Weapons;

public class NoneWeapon : BaseWeapon, IPrimaryWeapon
{
    public NoneWeapon(
        IProjectile baseProjectile = null,
        string name = "Placeholder weapon", 
        string description = "Does nothing.", 
        float baseCooldown = 0.5f, 
        int baseDamage = 1
    ) : base(name, description, baseCooldown, baseDamage, baseProjectile)
    {
    }
    
    public override void Fire(Vector2 pos, Vector2 direction, ProjectileStats stats)
    {
    }
}