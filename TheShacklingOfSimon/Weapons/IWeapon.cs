#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Entities.Projectiles.Augmentation;

#endregion

namespace TheShacklingOfSimon.Weapons;

public interface IWeapon
{
    string Name { get; }
    string Description { get; }
    float BaseCooldown { get; }
    int BaseDamage { get; }
    string SFX { get; }

    /// <summary>
    /// Retrieves the potentially decorated prototype projectile associated with this weapon.
    /// </summary>
    /// <returns>An instance of <see cref="IProjectile"/> representing the prototype projectile.</returns>
    IProjectile GetPrototype();

    /// <summary>
    /// Sets the prototype projectile to be used by this weapon.
    /// </summary>
    /// <remarks>
    /// If the specified newPrototype is decorated, these decorators will persist no matter what.
    /// </remarks>
    /// <param name="newPrototype">An instance of <see cref="IProjectile"/> representing the new prototype projectile.</param>
    void SetPrototype(IProjectile newPrototype);

    /// <summary>
    /// Fires a projectile from the weapon starting at the specified position,
    /// moving in the given direction, using the provided projectile statistics,
    /// and using the .
    /// </summary>
    /// <param name="pos">The initial position from which the projectile is fired.</param>
    /// <param name="direction">The direction in which the projectile will travel.</param>
    /// <param name="stats">The statistics defining the properties of the fired projectile, such as damage, speed, and owner type.</param>
    void Fire(Vector2 pos, Vector2 direction, ProjectileStats stats);

    bool AddAugment(IProjectileAugment augment);
    bool RemoveAugment(IProjectileAugment augment);
    void RebuildProjectile();

    event Action<IProjectile> OnProjectileFired;
}