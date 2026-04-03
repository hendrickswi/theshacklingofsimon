#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Factory;

#endregion

namespace TheShacklingOfSimon.Entities.Players.States.Body;

public class PlayerBodyIdleState : IPlayerBodyState
{
    private PlayerWithTwoSprites _player;
    
    public PlayerBodyIdleState(PlayerWithTwoSprites player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.Velocity = Vector2.Zero;

        string spritePrefix = _player.GetSkin("Body");
        _player.BodySprite = SpriteFactory.Instance.CreateStaticSprite(spritePrefix + "Idle");
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
        _player.BodySprite?.Update(delta);
    }

    public void HandleMovement(Vector2 direction, float frameDuration)
    {
        if (direction != Vector2.Zero)
        {
            _player.Velocity = direction;
            _player.StateManager.ChangeBodyState(new PlayerBodyMovingState(_player, frameDuration));
        }
    }
}