using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;

namespace TheShacklingOfSimon.Weapons;

public class BasicWeapon : BaseWeapon
{
	public BasicWeapon(IProjectile prototype)
	{
		Name = "Basic Weapon";
		Description = "Fires a simple projectile.";
		Prototype = prototype;
	}
}