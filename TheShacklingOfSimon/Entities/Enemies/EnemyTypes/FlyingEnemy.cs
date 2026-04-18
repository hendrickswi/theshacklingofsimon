#region

using Microsoft.Xna.Framework;
using System;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours.AttackBehaviours;
using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours.MovementBehaviours;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.Obstacles;
using TheShacklingOfSimon.Weapons;
using Vector2 = Microsoft.Xna.Framework.Vector2;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public class FlyingEnemy : BaseEnemy
{
    private readonly IAttackBehaviour _rangedAttackBehaviour;
    private readonly IAttackBehaviour _contactAttackBehaviour;

    public FlyingEnemy(Vector2 startPosition, IWeapon weapon, string name = "BlackMaw")
        : base(startPosition, weapon, name)
    {
        _rangedAttackBehaviour = new NoAttackBehaviour();
        _contactAttackBehaviour = new AllowedAttackBehaviour();
        _movementBehaviour = new ChaseMovementBehavior();
    }

    public override Vector2 FindTarget()
    {
        if (_targetPlayer == null)
        {
            return Vector2.Zero;
        }

        if (_pathfindingService == null)
        {
            Vector2 direct = _targetPlayer.Position - Position;
            if (direct.LengthSquared() < 0.0001f) return Vector2.Zero;
            direct.Normalize();
            return direct;
        }

        Func<ITile, bool> canTraverse = tile => !tile.BlocksFly;

        Func<ITile, float> getTraversalCost = tile =>
        {
            if (tile == null) return 1f;

            if (tile.BlocksFly)
            {
                return float.PositiveInfinity;
            }

            return tile switch
            {
                SpikeTile => 6f,
                FireTile => 8f,
                HoleTile => 1f,
                _ => 1f
            };
        };

        return _pathfindingService.GetNextDirection(
            Position,
            _targetPlayer.Position,
            canTraverse,
            getTraversalCost);
    }

    public override void RegisterAttack(float dt, Vector2 targetDirection)
    {
        _rangedAttackBehaviour.Execute(this, dt, targetDirection);
    }

    public override void OnCollision(IPlayer player)
    {
        if (player == null || !IsActive) return;

        _contactAttackBehaviour.Execute(this, 0, Vector2.Zero);
        player.TakeDamage((int)ContactDamage);
    }

    public override void OnCollision(ITile tile)
    {
        if (tile == null || !tile.BlocksFly) return;

        Vector2 mtv = CollisionDetector.CalculateMinimumTranslationVector(Hitbox, tile.Hitbox);
        if (mtv == Vector2.Zero) return;

        Position += mtv;
        Hitbox = new Rectangle((int)Position.X, (int)Position.Y, Hitbox.Width, Hitbox.Height);

        switch (CollisionDetector.GetCollisionSideFromMtv(mtv))
        {
            case CollisionSide.Left:
            case CollisionSide.Right:
                Velocity = new Vector2(0.0f, Velocity.Y);
                break;

            case CollisionSide.Top:
            case CollisionSide.Bottom:
                Velocity = new Vector2(Velocity.X, 0.0f);
                break;
        }
    }
}