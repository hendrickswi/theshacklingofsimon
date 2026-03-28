using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Weapons;

public class BombWeapon : BaseWeapon
{
    public BombWeapon(IProjectile prototype)
    {
        Name = "Bomb";
        Description = "Drops a bomb that explodes.";
        Prototype = prototype;
    }
}