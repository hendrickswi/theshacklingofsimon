using Vector2 = Microsoft.Xna.Framework.Vector2;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

namespace TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours;

public class ProjectileAttackBehaviour : IAttackBehaviour
{
    public void Execute(BaseEnemy enemy, float dt, Vector2 targetDirection)
{
    enemy.UpdateAttackTimer(dt);

    Vector2 direction = enemy.Velocity;

    if (enemy.CanAttack() && direction != Vector2.Zero)
    {
        enemy.CurrentState.HandleAttack(direction, enemy.AttackCooldown);
        enemy.ResetAttackTimer();
    }
}
}