#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours;

public class WanderThenChaseBehavior : IMovementBehavior
{
    private readonly WanderMovementBehavior _wander = new();
    private readonly ChaseMovementBehavior _chase = new();

    public Vector2 GetMovement(BaseEnemy enemy, float dt, Vector2 targetDirection)
    {
        if (targetDirection.LengthSquared() > enemy.AttackRange)
            return _wander.GetMovement(enemy, dt, targetDirection);

        return _chase.GetMovement(enemy, dt, targetDirection);
    }
}