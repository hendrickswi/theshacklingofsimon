using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players.States;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon.Entities.Players;

public interface IPlayer : IDamageable
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
    
    Inventory Inventory { get; }
    IWeapon CurrentWeapon { get; }
    IItem CurrentItem { get; }
    
    
    // IPlayer-implementing classes will act as the context for the State pattern
    IPlayerHeadState CurrentHeadState { get; }
    
    void AddWeaponToInventory(IWeapon weapon);

    IWeapon RemoveWeaponFromInventory(int pos);

    void AddItemToInventory(IItem item);

    IItem RemoveItemFromInventory(int pos);
    
    // pos is the index of the IItem in the player's Inventory
    void EquipItem(int pos);
    
    // pos is the index of the IWeapon in the player's Inventory
    void EquipWeapon(int pos);
    
    void Attack(Vector2 direction);
    void AttackSecondary(Vector2 direction);
    void RegisterMoveInput(Vector2 direction);
    
    void ChangeHeadState(IPlayerHeadState newHeadState);
    void ChangeBodyState(IPlayerBodyState newBodyState);
}