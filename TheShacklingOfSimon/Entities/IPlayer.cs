using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Entities;

public interface IPlayer : IDamageable
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
    /*
     * Inherits
     * Health { get; set; },
     * MaxHealth { get; set; },
     * void TakeDamage(float amt),
     * void Heal(float amt),
     * void Die()
     * from IDamageable
     */ 
    
    Inventory Inventory { get; }
    IWeapon CurrentWeapon { get; }
    IItem CurrentItem { get; }
    
    // Data packet passed to Weapons as damage modifiers
    PlayerStats CurrentStats { get; }
    // IPlayer-implementing classes will act as the context for the State pattern
    IPlayerState CurrentState { get; }
    
    void EquipItem();
    void EquipWeapon();
    void Attack(Vector2 direction);
    void Move(Vector2 direction);

    void ChangeState(IPlayerState newState);
}