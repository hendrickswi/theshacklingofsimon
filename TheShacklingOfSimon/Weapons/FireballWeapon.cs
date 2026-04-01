#region

using TheShacklingOfSimon.Entities.Projectiles;

#endregion

namespace TheShacklingOfSimon.Weapons;

public class FireballWeapon : BaseWeapon, ISecondaryWeapon
{
	public FireballWeapon(IProjectile prototype)
	{
		Name = "Fireball Weapon";
		Description = "Fires a fireball projectile.";
		Prototype = prototype;
	}
}