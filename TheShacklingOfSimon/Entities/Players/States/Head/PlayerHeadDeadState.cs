using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Entities.Players.States.Head;

public class PlayerHeadDeadState : IPlayerHeadState
{
    private readonly PlayerWithTwoSprites _player;

    public PlayerHeadDeadState(PlayerWithTwoSprites player)
    {
        _player = player;
    }
    
    public void Enter()
    {
        _player.HeadSprite = null;
    }

    public void Exit()
    {
        /*
         * No-op for now
         * Could use this later for stopping any sounds related to moving (e.g., walking)
         */
    }

    public void Update(GameTime delta)
    {
        // No-op
    }

    public void HandlePrimaryAttack(Vector2 direction, float stateDuration)
    {
        // No-op
    }

    public void HandleSecondaryAttack(Vector2 direction, float stateDuration)
    {
        // No-op
    }
}