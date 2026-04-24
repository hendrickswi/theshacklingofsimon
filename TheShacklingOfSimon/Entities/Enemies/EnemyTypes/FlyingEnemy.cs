#region

using Microsoft.Xna.Framework;
using System;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours.AttackBehaviours;
using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours.MovementBehaviours;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Rooms_and_Tiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles;
using TheShacklingOfSimon.Weapons;
using Vector2 = Microsoft.Xna.Framework.Vector2;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public class FlyingEnemy : BaseEnemy
{
    private const float WaypointReachDistance = 4f;

    private readonly IAttackBehaviour _rangedAttackBehaviour;
    private readonly IAttackBehaviour _contactAttackBehaviour;

    private Vector2? _currentFleeWaypoint;

    public FlyingEnemy(Vector2 startPosition, IWeapon weapon, string name = "AngelicBaby")
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
            _currentFleeWaypoint = null;
            return Vector2.Zero;
        }

        // Only the angel enemy flees. Other flying enemies can still chase normally.
        if (Name != "AngelicBaby")
        {
            return base.FindTarget();
        }

        Vector2 myCenter = GetHitboxCenter(Hitbox);
        Vector2 playerCenter = GetHitboxCenter(_targetPlayer.Hitbox);

        if (_pathfindingService is not GridPathfindingService gridPathfindingService)
        {
            return GetDirectDirection(playerCenter, myCenter);
        }

        if (_currentFleeWaypoint.HasValue)
        {
            float distanceSquared = Vector2.DistanceSquared(myCenter, _currentFleeWaypoint.Value);

            if (distanceSquared <= WaypointReachDistance * WaypointReachDistance)
            {
                _currentFleeWaypoint = null;
            }
        }

        if (!_currentFleeWaypoint.HasValue)
        {
            bool foundWaypoint = gridPathfindingService.TryGetNextWaypointAwayFromThreat(
                myCenter,
                playerCenter,
                CanTraverseTile,
                GetTraversalCost,
                out Vector2 nextWaypoint
            );

            if (!foundWaypoint)
            {
                return Vector2.Zero;
            }

            _currentFleeWaypoint = nextWaypoint;
        }

        return GetDirectDirection(myCenter, _currentFleeWaypoint.Value);
    }

    protected override bool CanTraverseTile(ITile tile)
    {
        if (tile == null)
        {
            return true;
        }

        // Flying enemies can travel over fire, spikes, and holes.
        // They only stop at tiles that specifically block flying.
        return !tile.BlocksFly;
    }

    protected override float GetTraversalCost(ITile tile)
    {
        if (tile == null)
        {
            return 1f;
        }

        if (!CanTraverseTile(tile))
        {
            return float.PositiveInfinity;
        }

        // Fire, spikes, and holes are normal terrain for flying enemies.
        return 1f;
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
        _currentFleeWaypoint = null;

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