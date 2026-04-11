#region

using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;
using Vector2 = Microsoft.Xna.Framework.Vector2;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours.AttackBehaviours;

public class AllowedAttackBehaviour : IAttackBehaviour
{
    public void Execute(BaseEnemy enemy, float dt, Vector2 targetDirection)
    {
        enemy.UpdateAttackTimer(dt);

        Vector2 direction = enemy.Velocity;

        if (enemy.CanAttack() && direction != Vector2.Zero)
        {
            enemy.CurrentState.HandleAttack(direction, 0.2f);
            enemy.ResetAttackTimer();
        }
    }
}