#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours;

public class NoMovementBehaviour : IMovementBehavior
{
    public Vector2 GetMovement(BaseEnemy enemy, float dt, Vector2 targetDirection)
    {
        Vector2 direction = Vector2.Zero;
        
        return direction;
    }
}