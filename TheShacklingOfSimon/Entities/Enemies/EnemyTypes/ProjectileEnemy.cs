using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies.States;

namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public interface IProjectileEnemy : IEnemy
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
}