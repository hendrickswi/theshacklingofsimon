#region

using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sounds;

#endregion

namespace TheShacklingOfSimon.Weapons;

public class BombWeapon : BaseWeapon, ISecondaryWeapon
{
    public BombWeapon(IProjectile prototype)
    {
        Name = "Bomb";
        Description = "Drops a bomb that explodes.";
        BaseCooldown = 1.25f;
        BaseDamage = 3;
        Prototype = prototype;
    }
}