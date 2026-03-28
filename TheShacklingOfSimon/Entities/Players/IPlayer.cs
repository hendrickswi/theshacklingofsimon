using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players.States;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon.Entities.Players;

public interface IPlayer : IDamageable
{
    PlayerInventory Inventory { get; }
    
    // IPlayer-implementing classes will act as the context for the State pattern
    IPlayerState CurrentState { get; }
    
    float MoveSpeedStat { get; set; }
    int DamageMultiplierStat { get; set; }
    float ProjectileSpeedMultiplierStat { get; set; }
    float PrimaryAttackCooldown { get; set; }
    float SecondaryAttackCooldown { get; set; }
    
    void RegisterMoveInput(Vector2 direction);
    void RegisterPrimaryAttackInput(Vector2 direction);
    void RegisterSecondaryAttackInput(Vector2 direction);
    
    void Reset(Vector2 startPosition);
    void SetSkin(string category, string skinPrefix);
    string GetSkin(string category);
    
    void ChangeState(IPlayerState newState);
}
