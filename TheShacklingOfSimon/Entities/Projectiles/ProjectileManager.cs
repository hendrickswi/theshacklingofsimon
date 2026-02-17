using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Entities.Projectiles;

public class ProjectileManager
{
	private List<IProjectile> _projectiles;

	public ProjectileManager()
	{
		_projectiles = new List<IProjectile>();
	}

	public void AddProjectile(IProjectile projectile)
	{
		_projectiles.Add(projectile);
	}

	public void Update(GameTime gameTime)
	{
		foreach (var p in _projectiles)
		{
			if (p.IsActive)
				p.Update(gameTime);
		}

		_projectiles.RemoveAll(p => !p.IsActive);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		foreach (var p in _projectiles)
		{
			if (p.IsActive)
				p.Draw(spriteBatch);
		}
	}
}