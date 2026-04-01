#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours;

public class WanderMovementBehavior : IMovementBehavior
{
    private static Random _rng = new Random();

    private float _wanderTimer;
    private float _wanderInterval;
    private Vector2 _wanderDirection;

    public WanderMovementBehavior(float interval = 1.5f)
    {
        _wanderInterval = interval;
        _wanderTimer = 0f;
    }

    public Vector2 GetMovement(BaseEnemy enemy, float dt, Vector2 targetDirection)
    {
        _wanderTimer -= dt;

        if (_wanderTimer <= 0f)
        {
            float angle = (float)(_rng.NextDouble() * Math.PI * 2);

            _wanderDirection = new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle)
            );

            _wanderTimer = _wanderInterval;
        }

        return _wanderDirection;
    }
}