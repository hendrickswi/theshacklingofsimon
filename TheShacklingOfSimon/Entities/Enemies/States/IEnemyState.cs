using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Entities.Enemies.States;

public interface IEnemyState
{
    void Enter();
    void Exit();
    void Update(GameTime delta);
    void HandleMovement(Vector2 movementInput);
    void HandleAttack(Vector2 attackInput, float attackDamage, float attackCooldown, float attackRange);
}