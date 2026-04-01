#region

using TheShacklingOfSimon.Entities.Projectiles;

#endregion

namespace TheShacklingOfSimon.Weapons;

public class BombWeapon : BaseWeapon, ISecondaryWeapon
{
    public BombWeapon(IProjectile prototype)
    {
        Name = "Bomb";
        Description = "Drops a bomb that explodes.";
        Prototype = prototype;
    }
}