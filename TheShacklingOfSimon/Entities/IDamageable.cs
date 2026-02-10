namespace TheShacklingOfSimon.Entities;

public interface IDamageable : IEntity
{
    /*
     * Inherits
     * Vector2 Position { get; set; }
     * Rectangle Hitbox { get; set; }
     * void SetSprite(SpriteType type),
     * void Update(GameTime delta),
     * void Draw(SpriteBatch spriteBatch)
     * from IEntity
     */
    
    int Health { get; }
    int MaxHealth { get; }

    void TakeDamage(int amt);
    void Heal(int amt);
    void Die();
}