using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Projectiles;

namespace TheShacklingOfSimon.Weapons;

public class BasicWeapon : IWeapon
{
	private ProjectileManager _projectileManager;

	public string Name { get; private set; }
	public string Description { get; private set; }

	public BasicWeapon(ProjectileManager manager)
	{
		_projectileManager = manager;

		Name = "Basic Weapon";
		Description = "Fires a simple projectile.";
	}

	public void Fire(Vector2 pos, Vector2 direction, ProjectileStats stats)
	{
		var projectile = new BasicProjectile(pos, direction, stats);
		_projectileManager.AddProjectile(projectile);
	}
}