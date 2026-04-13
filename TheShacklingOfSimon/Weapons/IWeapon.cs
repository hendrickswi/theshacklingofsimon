#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;

#endregion

namespace TheShacklingOfSimon.Weapons;

public interface IWeapon
{
    string Name { get; }
    string Description { get; }
    float BaseCooldown { get; }
    int BaseDamage { get; }
    string SFX { get; }

    IProjectile GetPrototype();
    void SetPrototype(IProjectile newPrototype);
    void Fire(Vector2 pos, Vector2 direction, ProjectileStats stats);
    event Action<IProjectile> OnProjectileFired;
}