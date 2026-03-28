using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Weapons;
using TheShacklingOfSimon.Weapons.WeaponTypeList;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Entities.Enemies.Managers;
public static class EnemyWeaponFactory
{
    public static IWeapon CreateWeapon(WeaponTypeList type)
    {
        switch (type)
        {
            case WeaponTypeList.NoneWeapon:
                return new NoneWeapon();

            case WeaponTypeList.BasicWeapon:
                return new BasicWeapon(
                    new BasicProjectile(
                        Vector2.Zero,
                        new Vector2(0,1),
                        SpriteFactory.Instance.CreateStaticSprite("BasicProjectile"),
                        new ProjectileStats(1, 200f, ProjectileOwner.Enemy)
                    )
                );

            case WeaponTypeList.BombWeapon:
                return new BombWeapon(
                    new BasicProjectile(
                        Vector2.Zero,
                        new Vector2(0,1),
                        SpriteFactory.Instance.CreateStaticSprite("BasicProjectile"),
                        new ProjectileStats(1, 200f, ProjectileOwner.Enemy)
                    )
                );

            default:
                return null;
        }
    }
}