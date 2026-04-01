#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;

#endregion

namespace TheShacklingOfSimon.Weapons;

public class NoneWeapon : BaseWeapon, IPrimaryWeapon
{
    public NoneWeapon()
    {
        Name = "NoneWeapon";
        Description = "Placeholder weapon";
        Prototype = null;
    }

    
    public override void Fire(Vector2 pos, Vector2 direction, ProjectileStats stats)
    {
        // No-op
    }
}