using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies.States;

namespace TheShacklingOfSimon.Entities.Enemies;

public interface IEnemy : IDamageable
{
    /*
     * Inherits
     * Vector2 Position { get; }
     * Vector2 Velocity { get; }
     * bool IsActive { get; }
     * Rectangle Hitbox { get; }
     * ISprite Sprite { get; }
     * 
     * void Update(GameTime delta),
     * void Draw(SpriteBatch spriteBatch),
     * void Discontinue();
     *
     * To be implemented after Sprint 2:
     * void Interact(IEntity other)
     * 
     * from IEntity
     */
    /*
     * Inherits
     * Health { get; },
     * MaxHealth { get; },
     * void TakeDamage(float amt),
     * void Heal(float amt)
     * from IDamageable
     */
    
    // IEnemy-implementing classes will act as the context for the State pattern
    IEnemyState CurrentState { get; }
    
    float MoveSpeedStat { get; set; }
    float AttackCooldown { get; set; }
    float AttackRange { get; set; }

    // are attacks specialized to each enemy or considered weapons
    //void Attack(Vector2 direction, float attackDamage, float attackCooldown, float attackRange);
    Vector2 FindTarget();
    void Pathfind(Vector2 targetPosition);
    void RegisterAttack(Vector2 direction);
    void ChangeState(IEnemyState newState);
}