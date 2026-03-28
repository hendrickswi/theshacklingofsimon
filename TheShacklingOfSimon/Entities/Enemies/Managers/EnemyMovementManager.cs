using System;
using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Entities.Enemies
{
    public class EnemyMovementManager
    {
        private static Random _rng = new Random();

        private float _wanderTimer;
        private float _wanderInterval;
        private Vector2 _wanderDirection;

        public EnemyMovementManager(float wanderInterval = 1.5f)
        {
            _wanderInterval = wanderInterval;
            _wanderTimer = 0f;
            _wanderDirection = Vector2.Zero;
        }

        public Vector2 Wander(float dt)
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

        public Vector2 Pathfind(Vector2 targetDirection)
        {
            if (targetDirection.LengthSquared() < 0.0001f)
                return Vector2.Zero;

            Vector2 direction = targetDirection;
            direction.Normalize();

            return direction;
        }
    }
}