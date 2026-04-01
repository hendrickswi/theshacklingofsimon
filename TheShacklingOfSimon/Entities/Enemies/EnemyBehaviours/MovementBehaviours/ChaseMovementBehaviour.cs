#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours;

public class ChaseMovementBehavior : IMovementBehavior
{
    public Vector2 GetMovement(BaseEnemy enemy, float dt, Vector2 targetDirection)
    {
        if (targetDirection.LengthSquared() < 0.0001f)
            return Vector2.Zero;

        Vector2 direction = targetDirection;
        direction.Normalize();

        return direction;
    }
}