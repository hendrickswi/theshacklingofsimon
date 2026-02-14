using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Entities.Players.States.Head;

public interface IPlayerHeadState : IPlayerState
{
    /*
     * Inherits
     * void Enter(),
     * void Exit(),
     * void Update(GameTime delta)
     * from IPlayerState
     */
    
    void HandlePrimaryAttack(Vector2 direction, float stateDuration);
    void HandleSecondaryAttack(Vector2 direction, float stateDuration);
}