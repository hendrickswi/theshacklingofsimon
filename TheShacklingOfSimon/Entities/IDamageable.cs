namespace TheShacklingOfSimon.Entities;

public interface IDamageable : IEntity
{
    /*
     * Inherits
     * Vector2 Position { get; set; }
     * Vector2 Velocity { get; set; }
     * bool IsActive { get; set; }
     * Rectangle Hitbox { get; set; }
     * ISprite Sprite { get; set; }
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
    
    int Health { get; }
    int MaxHealth { get; }

    void TakeDamage(int amt);
    void Heal(int amt);
}