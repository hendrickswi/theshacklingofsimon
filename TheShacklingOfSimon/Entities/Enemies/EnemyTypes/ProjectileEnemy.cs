#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours.AttackBehaviours;
using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours.MovementBehaviours;
using TheShacklingOfSimon.Rooms_and_Tiles;
using TheShacklingOfSimon.Weapons;
using Vector2 = Microsoft.Xna.Framework.Vector2;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public class ProjectileEnemy : BaseEnemy
{
    private const float AlignmentTolerance = 12f;
    private const float WaypointReachDistance = 4f;

    private readonly IAttackBehaviour _attackBehaviour;

    private Vector2? _currentAlignmentWaypoint;

    public ProjectileEnemy(Vector2 startPosition, IWeapon weapon, string name = "SpiderEnemy")
        : base(startPosition, weapon, name)
    {
        _attackBehaviour = new AllowedAttackBehaviour();

        // Spiders should move intentionally toward shooting positions.
        _movementBehaviour = new ChaseMovementBehavior();
    }

    public override Vector2 FindTarget()
    {
        if (_targetPlayer == null)
        {
            _currentAlignmentWaypoint = null;
            return Vector2.Zero;
        }

        Vector2 myCenter = GetHitboxCenter(Hitbox);
        Vector2 playerCenter = GetHitboxCenter(_targetPlayer.Hitbox);

        if (HasClearShot(myCenter, playerCenter))
        {
            _currentAlignmentWaypoint = null;
            return Vector2.Zero;
        }

        if (_pathfindingService is not GridPathfindingService gridPathfindingService)
        {
            return GetDirectDirection(myCenter, playerCenter);
        }

        if (_currentAlignmentWaypoint.HasValue)
        {
            float distanceSquared = Vector2.DistanceSquared(myCenter, _currentAlignmentWaypoint.Value);

            if (distanceSquared <= WaypointReachDistance * WaypointReachDistance)
            {
                _currentAlignmentWaypoint = null;
            }
        }

        if (!_currentAlignmentWaypoint.HasValue)
        {
            bool foundWaypoint = gridPathfindingService.TryGetNextWaypointToAlignmentWithTarget(
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

            _currentAlignmentWaypoint = nextWaypoint;
        }

        return GetDirectDirection(myCenter, _currentAlignmentWaypoint.Value);
    }

    public override void RegisterAttack(float dt, Vector2 targetDirection)
    {
        UpdateAttackTimer(dt);

        if (_targetPlayer == null || !CanAttack())
        {
            return;
        }

        Vector2 myCenter = GetHitboxCenter(Hitbox);
        Vector2 playerCenter = GetHitboxCenter(_targetPlayer.Hitbox);

        if (!HasClearShot(myCenter, playerCenter))
        {
            return;
        }

        Vector2 shootDirection = GetShootDirection(myCenter, playerCenter);

        if (shootDirection == Vector2.Zero)
        {
            return;
        }

        CurrentState.HandleAttack(shootDirection, 0.2f);
        ResetAttackTimer();
    }

    protected override Point GetGameplayHitboxSize()
    {
        // Spiders use a slightly smaller hitbox than Black Maw so they have
        // more room to path around spikes, fire, rocks, and holes.
        return new Point(22, 22);
    }

    private bool HasClearShot(Vector2 myCenter, Vector2 playerCenter)
    {
        if (!IsAlignedWithPlayer(myCenter, playerCenter))
        {
            return false;
        }

        if (_pathfindingService is not GridPathfindingService gridPathfindingService)
        {
            return true;
        }

        return gridPathfindingService.HasClearShot(myCenter, playerCenter);
    }

    private static bool IsAlignedWithPlayer(Vector2 myCenter, Vector2 playerCenter)
    {
        return Math.Abs(myCenter.X - playerCenter.X) <= AlignmentTolerance ||
               Math.Abs(myCenter.Y - playerCenter.Y) <= AlignmentTolerance;
    }

    private static Vector2 GetShootDirection(Vector2 myCenter, Vector2 playerCenter)
    {
        Vector2 difference = playerCenter - myCenter;

        if (difference.LengthSquared() < 0.0001f)
        {
            return Vector2.Zero;
        }

        if (Math.Abs(difference.X) > Math.Abs(difference.Y))
        {
            return new Vector2(Math.Sign(difference.X), 0f);
        }

        return new Vector2(0f, Math.Sign(difference.Y));
    }
}