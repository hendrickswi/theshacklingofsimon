using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;

namespace TheShacklingOfSimon.Weapons;

public class BasicWeapon : IWeapon
{
	public string Name { get; private set; }
	public string Description { get; private set; }

	private IProjectile _prototype;

	public BasicWeapon(IProjectile prototype)
	{
		Name = "Basic Weapon";
		Description = "Fires a simple projectile.";
		_prototype = prototype;
	}

	public void Fire(Vector2 pos, Vector2 direction, ProjectileStats stats)
	{
		var firedProjectile = _prototype.Clone(pos, direction, _prototype.Sprite, stats);
		OnProjectileFired?.Invoke(firedProjectile);
	}

	public event Action<IProjectile> OnProjectileFired;
}