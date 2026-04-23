#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.StatusEffects;

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

        string spritePrefix = _player.SpritesManager.GetSkin("Body");
        _player.SpritesManager.Body = SpriteFactory.Instance.CreateStaticSprite(spritePrefix + "Idle");
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
    }

    public void HandleMovement(Vector2 direction, float frameDuration)
    {
        if (direction.LengthSquared() < float.Epsilon 
            && Math.Abs(_player.GetStat(StatType.MoveSpeed)) < float.Epsilon 
            && Math.Abs(_player.GetStat(StatType.MoveSpeedMultiplier)) < float.Epsilon 
            && _player.GetStat(StatType.StunCount) > float.Epsilon
            )
            return;
        
        // Only transition to moving state if the player should actually be moving
        _player.Velocity = direction;
        _player.StatesManager.ChangeBodyState(new PlayerBodyMovingState(_player, frameDuration));
    }
}