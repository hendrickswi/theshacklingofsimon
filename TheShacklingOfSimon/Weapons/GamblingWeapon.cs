#region

using Microsoft.Xna.Framework;
using System;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Weapons;

public class GamblingWeapon : BaseWeapon, IPrimaryWeapon
{
    private IProjectile _prototype;
    public GamblingWeapon(
		IProjectile baseProjectile,
		string name = "Gambling Weapon", 
		string description = "Fires a projectile with a 1/5 chance of killing the target", 
		float baseCooldown = 1f, 
		int baseDamage = 0
	) : base(name, description, baseCooldown, baseDamage, baseProjectile)
	{
        SFX = SoundManager.Instance.AddSFX("items", "plop");
	}

    public override void Fire(Vector2 pos, Vector2 direction, ProjectileStats stats)
    {   
        _prototype=GetCurrentPrototype();
        Random random = new Random();
        if (random.Next(1, 6) == 1)
        {
            stats.Damage = 1000;
           
            _prototype.Sprite = SpriteFactory.Instance.CreateStaticSprite("WinGamble");


        }
        else
        {
            stats.Damage = 0;
            _prototype.Sprite = SpriteFactory.Instance.CreateStaticSprite("LoseGamble");

        }
       base.Fire(pos, direction, stats);
    }
}