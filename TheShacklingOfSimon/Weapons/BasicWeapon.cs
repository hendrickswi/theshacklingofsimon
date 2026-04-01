#region

using TheShacklingOfSimon.Entities.Projectiles;

#endregion

namespace TheShacklingOfSimon.Weapons;

public class BasicWeapon : BaseWeapon, IPrimaryWeapon
{
	public BasicWeapon(IProjectile prototype)
	{
		Name = "Basic Weapon";
		Description = "Fires a simple projectile.";
		Prototype = prototype;
	}
}