using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Weapons;

public class BombWeapon : IWeapon
{
    public string Name { get; }
    public string Description { get; }

    private IProjectile _prototype;

    public BombWeapon(IProjectile prototype)
    {
        Name = "Bomb";
        Description = "Drops a bomb that explodes.";

        _prototype = prototype;
    }

    public void Fire(Vector2 pos, Vector2 direction, ProjectileStats stats)
    {
        var firedProjectile = _prototype.Clone(pos, direction, _prototype.Sprite, stats);
        OnProjectileFired?.Invoke(firedProjectile);
    }

    public event Action<IProjectile> OnProjectileFired;
}