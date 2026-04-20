#region

using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sounds;

#endregion

namespace TheShacklingOfSimon.Weapons;

public class BasicWeapon : BaseWeapon, IPrimaryWeapon
{
	public BasicWeapon(
		IProjectile baseProjectile,
		string name = "Basic Weapon", 
		string description = "Fires a simple projectile.", 
		float baseCooldown = 0.5f, 
		int baseDamage = 1
	) : base(name, description, baseCooldown, baseDamage, baseProjectile)
	{
        SFX = SoundManager.Instance.AddSFX("items", "plop");
	}
}