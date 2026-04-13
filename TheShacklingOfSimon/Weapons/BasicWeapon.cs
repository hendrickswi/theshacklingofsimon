#region

using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sounds;

#endregion

namespace TheShacklingOfSimon.Weapons;

public class BasicWeapon : BaseWeapon, IPrimaryWeapon
{
	public BasicWeapon(IProjectile prototype)
	{
		Name = "Basic Weapon";
		Description = "Fires a simple projectile.";
		BaseCooldown = 0.5f;
		BaseDamage = 1;
		Prototype = prototype;
		SFX = SoundManager.Instance.NameSFX("items", "plop");
        SoundManager.Instance.AddSFX(SFX);
	}
}