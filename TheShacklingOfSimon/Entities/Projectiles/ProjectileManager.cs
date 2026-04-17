#region

using System;
using System.Collections.Generic;

#endregion

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
		OnProjectileAdded?.Invoke(projectile);
	}

    public void Clear()
    {
        _projectiles.Clear();
    }
    
    public event Action<IProjectile> OnProjectileAdded;
}