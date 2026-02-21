using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players.States;
using TheShacklingOfSimon.Items;
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
    IWeapon CurrentPrimaryWeapon { get; }
    IWeapon CurrentSecondaryWeapon { get; }
    IItem CurrentItem { get; }
    
    // IPlayer-implementing classes will act as the context for the State pattern
    IPlayerState CurrentState { get; }
    
    float MoveSpeedStat { get; set; }
    float DamageMultiplierStat { get; set; }
    float ProjectileSpeedMultiplierStat { get; set; }
    float PrimaryAttackCooldown { get; set; }
    float SecondaryAttackCooldown { get; set; }
    
    void AddWeaponToInventory(IWeapon weapon);

    IWeapon RemoveWeaponFromInventory(int pos);

    void AddItemToInventory(IItem item);

    IItem RemoveItemFromInventory(int pos);
    
    void EquipItem(int pos);
    void EquipPrimaryWeapon(int pos);
    void EquipSecondaryWeapon(int pos);
    
    void RegisterMoveInput(Vector2 direction);
    void RegisterPrimaryAttackInput(Vector2 direction);
    void RegisterSecondaryAttackInput(Vector2 direction);

    void TeleportTo(Vector2 worldPosition);

    void ChangeState(IPlayerState newState);
}