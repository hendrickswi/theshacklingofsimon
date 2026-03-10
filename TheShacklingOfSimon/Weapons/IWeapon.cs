using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;

namespace TheShacklingOfSimon.Weapons;

public interface IWeapon
{
    string Name { get; }
    string Description { get; }
    
    void Fire(Vector2 pos, Vector2 direction, ProjectileStats stats);
    event Action<IProjectile> OnProjectileFired;
}