#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours.MovementBehaviours;

public enum MovementAxis
{
    X,
    Y
}

public class OneAxisPatrolMovementBehavior : IMovementBehavior
{
    private readonly MovementAxis _axis;
    private int _direction = 1; // 1 for positive, -1 for negative
    private float _timer;
    private readonly float _patrolDuration;

    public OneAxisPatrolMovementBehavior(MovementAxis axis, float patrolDuration = 2.0f)
    {
        _axis = axis;
        _patrolDuration = patrolDuration;
        _timer = _patrolDuration;
    }

    public Vector2 GetMovement(BaseEnemy enemy, float dt, Vector2 targetDirection)
    {
        _timer -= dt;
        if (_timer <= 0)
        {
            _direction = -_direction;
            _timer = _patrolDuration;
        }

        Vector2 movement = Vector2.Zero;

        if (_axis == MovementAxis.X)
        {
            movement.X = _direction;
        }
        else
        {
            movement.Y = _direction;
        }

        return movement;
    }
}