#region

using TheShacklingOfSimon.Entities.Projectiles;

#endregion

namespace TheShacklingOfSimon.Weapons;

public class BombWeapon : BaseWeapon, ISecondaryWeapon
{
    public BombWeapon(
        IProjectile baseProjectile,
        string name = "Bomb", 
        string description = "Drops a bomb that explodes.", 
        float baseCooldown = 1.25f, 
        int baseDamage = 3
    ) : base(name, description, baseCooldown, baseDamage, baseProjectile)
    {
        // add sfx here
    }
}