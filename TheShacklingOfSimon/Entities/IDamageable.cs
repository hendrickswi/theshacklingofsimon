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
    
    float Health { get; set; }
    float MaxHealth { get; set; }

    void TakeDamage(float amt);
    void Heal(float amt);
    void Die();
}