using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Weapons;

public class BombWeapon : IWeapon
{
    private ProjectileManager _projectileManager;

    public string Name { get; }
    public string Description { get; }

    public BombWeapon(ProjectileManager manager)
    {
        _projectileManager = manager;

        Name = "Bomb";
        Description = "Drops a bomb that explodes.";
    }

    public void Fire(Vector2 pos, Vector2 direction, ProjectileStats stats)
    {
        
        var bombSprite = SpriteFactory.Instance.CreateAnimatedSprite("PlayerHeadShootingDown", 0.1f);

        var bomb = new BombProjectile(pos, bombSprite, stats);

        _projectileManager.AddProjectile(bomb);
    }
}