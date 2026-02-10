using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon.Entities.Players;

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
    
    
    // IPlayer-implementing classes will act as the context for the State pattern
    IPlayerState CurrentState { get; }
    
    void AddWeaponToInventory(IWeapon weapon);

    IWeapon RemoveWeaponFromInventory(int pos);

    void AddItemToInventory(IItem item);

    IItem RemoveItemFromInventory(int pos);
    
    // pos is the index of the IItem in the player's Inventory
    void EquipItem(int pos);
    
    // pos is the index of the IWeapon in the player's Inventory
    void EquipWeapon(int pos);
    
    void Attack(Vector2 direction);
    
    void Move(Vector2 direction);

    void ChangeState(IPlayerState newState);
}