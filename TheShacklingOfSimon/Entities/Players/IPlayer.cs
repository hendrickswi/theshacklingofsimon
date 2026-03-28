using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players.States;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon.Entities.Players;

public interface IPlayer : IDamageable
{
    PlayerInventory Inventory { get; }
    PlayerStats Stats { get; }
    PlayerInputBuffer InputBuffer { get; }
    
    // IPlayer-implementing classes will act as the context for the State pattern
    IPlayerState CurrentState { get; }
    
    void Reset(Vector2 startPosition);
    void SetSkin(string category, string skinPrefix);
    string GetSkin(string category);
    
    void ChangeState(IPlayerState newState);
}
