#region

using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sounds;

#endregion

namespace TheShacklingOfSimon.Weapons;

public class GamblingWeapon : BaseWeapon, IPrimaryWeapon
{
	public GamblingWeapon(
		IProjectile baseProjectile,
		string name = "Gambling Weapon", 
		string description = "Fires a projectile with a 1/10 chance of killing the target", 
		float baseCooldown = 1f, 
		int baseDamage = 0
	) : base(name, description, baseCooldown, baseDamage, baseProjectile)
	{
        SFX = SoundManager.Instance.AddSFX("items", "plop");
	}
}