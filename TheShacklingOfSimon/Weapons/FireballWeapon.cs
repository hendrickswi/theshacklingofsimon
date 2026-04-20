#region

using TheShacklingOfSimon.Entities.Projectiles;

#endregion

namespace TheShacklingOfSimon.Weapons;

public class FireballWeapon : BaseWeapon, IPrimaryWeapon
{
	public FireballWeapon(
		IProjectile baseProjectile,
		string name = "Fireball Spell", 
		string description = "Fires a fireball projectile.", 
		float baseCooldown = 0.67f, 
		int baseDamage = 2
	) : base(name, description, baseCooldown, baseDamage, baseProjectile)
	{
		// add sfx here
	}
}