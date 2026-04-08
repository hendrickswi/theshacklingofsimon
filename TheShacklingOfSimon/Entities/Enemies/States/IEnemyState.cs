#region

using Microsoft.Xna.Framework;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.States;

public interface IEnemyState
{
    void Enter();
    void Exit();
    void Update(GameTime delta);
    void HandleMovement(Vector2 movementInput);
    void HandleAttack(Vector2 direction, float stateDuration);
    void HandleDamage(int damage);
}