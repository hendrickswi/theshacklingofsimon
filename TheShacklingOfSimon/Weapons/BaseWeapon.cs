using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;

namespace TheShacklingOfSimon.Weapons;

public class BaseWeapon : IWeapon
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }

    protected IProjectile Prototype;
    
    public virtual void Fire(Vector2 pos, Vector2 direction, ProjectileStats stats)
    {
        var firedProjectile = Prototype.Clone(pos, direction, Prototype.Sprite, stats);
        OnProjectileFired?.Invoke(firedProjectile);
    }

    public void SetPrototype(IProjectile newPrototype)
    {
        if (newPrototype == null)
        {
            Console.WriteLine("null newPrototype passed into SetPrototype(IProjectile newPrototype).");
            return;
        }
        
        Prototype = newPrototype;
    }

    public event Action<IProjectile> OnProjectileFired;
}